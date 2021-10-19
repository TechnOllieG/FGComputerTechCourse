using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TechnOllieG
{
	public class TerrainGenerationSystem : SystemBase
	{
		private GameData _data;
		private List<int2> _chunksToGenerate = new List<int2>();
		private List<int2> _chunksToDegenerate = new List<int2>();
		private List<int2> _currentlyGeneratedChunks = new List<int2>();
		private EntityQueryDesc _objectsWithoutCoordinateComponentDesc;

		protected override void OnStartRunning()
		{
			_data = GetSingleton<GameData>();

			_objectsWithoutCoordinateComponentDesc = new EntityQueryDesc
			{
				None = new ComponentType[] {typeof(ChunkCoordinate)},
				All = new ComponentType[] {typeof(Translation), ComponentType.ReadOnly<BlockState>()}
			};
		}

		protected override void OnUpdate()
		{
			DetermineChunksToGenerate(ConvertPositionToChunkCoordinate(Vector3.zero));
			GenerateChunks();
			DegenerateChunks();
		}

		private void GenerateChunks()
		{
			if (_chunksToGenerate.Count == 0)
				return;

			foreach (int2 chunk in _chunksToGenerate)
			{
				GenerateChunk(chunk);
			}
			
			_chunksToGenerate.Clear();
		}

		private void DegenerateChunks()
		{
			if (_chunksToDegenerate.Count == 0)
				return;
			
			// todo implement
			
			_chunksToDegenerate.Clear();
		}

		private void GenerateChunk(int2 chunkCoordinates)
		{
			int chunkArea = _data.chunkSize * _data.chunkSize;
			NativeArray<Entity> grassBlocks = EntityManager.Instantiate(_data.grass, chunkArea, Allocator.Temp);
			NativeArray<Entity> dirtBlocks = EntityManager.Instantiate(_data.dirt, chunkArea * _data.layersOfDirt, Allocator.Temp);

			grassBlocks.Dispose();
			dirtBlocks.Dispose();
			
			int chunkSize = _data.chunkSize;
			int extents = _data.worldExtents;
			float xOffset = _data.xOffset;
			float yOffset = _data.yOffset;
			float scale = _data.scale;
			float depth = _data.depth;
			
			ChunkCoordinate coordinate;
			coordinate.chunkCoordinate = chunkCoordinates;

			EntityManager.AddSharedComponentData(GetEntityQuery(_objectsWithoutCoordinateComponentDesc), coordinate);

			Entities.WithSharedComponentFilter(coordinate)
				.ForEach((int entityInQueryIndex, ref Translation translation, in BlockState state) =>
				{
					int layer = entityInQueryIndex / chunkArea;
					int blockIndexInChunk = entityInQueryIndex % chunkArea;
					int2 chunkSpaceBlockCoordinate =
						new int2(blockIndexInChunk % chunkSize, blockIndexInChunk / chunkSize);

					int2 zeroIndexedChunkCoordinate = (int2) math.remap(new float2(-extents), new float2(extents),
						float2.zero,
						new float2(extents * 2), chunkCoordinates);

					int worldSizeInBlocks = (extents * 2 + 1) * chunkSize;
					float noiseX = (float) (zeroIndexedChunkCoordinate.x * chunkSize + chunkSpaceBlockCoordinate.x) / worldSizeInBlocks * scale + xOffset;
					float noiseY = (float) (zeroIndexedChunkCoordinate.y * chunkSize + chunkSpaceBlockCoordinate.y) / worldSizeInBlocks * scale + yOffset;

					float2 chunkCoord2d = chunkCoordinates * chunkSize + chunkSpaceBlockCoordinate;

					translation.Value.x = chunkCoord2d.x;
					translation.Value.y = math.round(Mathf.PerlinNoise(noiseX, noiseY) * depth - layer);
					translation.Value.z = chunkCoord2d.y;
				}).ScheduleParallel();
			
			_currentlyGeneratedChunks.Add(chunkCoordinates);
		}
		
		private void DetermineChunksToGenerate(int2 currentChunk)
		{

			int renderDistance = _data.renderDistance;
			List<int2> currentChunksToRender = new List<int2> {currentChunk};

			for (int i = 1; i <= renderDistance; i++)
			{
				int startOfCardinals = currentChunksToRender.Count;
				
				// Cardinal directions
				currentChunksToRender.Add(currentChunk.WithAdd(x: i));
				currentChunksToRender.Add(currentChunk.WithAdd(x: -i));
				currentChunksToRender.Add(currentChunk.WithAdd(y: i));
				currentChunksToRender.Add(currentChunk.WithAdd(y: -i));
				
				// Corners
				currentChunksToRender.Add(currentChunk.WithAdd(x: i, y: i));
				currentChunksToRender.Add(currentChunk.WithAdd(x: i, y: -i));
				currentChunksToRender.Add(currentChunk.WithAdd(x: -i, y: i));
				currentChunksToRender.Add(currentChunk.WithAdd(x: -i, y: -i));
				
				// Edges
				int2 baseCardinal = currentChunksToRender[startOfCardinals];
				int2 incrementAxis = new int2(0, 1);
				
				for (int j = 0; j < 4; j++)
				{
					for (int k = 1; k <= (renderDistance - 1) * 2; k++)
					{
						currentChunksToRender.Add(baseCardinal + incrementAxis * k);
						currentChunksToRender.Add(baseCardinal - incrementAxis * k);
					}
					
					baseCardinal = currentChunksToRender[++startOfCardinals];

					if (j == 1)
						incrementAxis = new int2(1, 0);
				}
			}

			foreach (int2 current in _currentlyGeneratedChunks)
			{
				if (currentChunksToRender.Contains(current))
					continue;
				
				_chunksToDegenerate.Add(current);
			}

			foreach (int2 current in currentChunksToRender)
			{
				if (_currentlyGeneratedChunks.Contains(current))
					continue;
				
				_chunksToGenerate.Add(current);
			}
		}
		
		int2 ConvertPositionToChunkCoordinate(Vector3 position)
		{
			return new int2((int) math.floor(position.x / _data.chunkSize), (int) math.floor(position.z / _data.chunkSize));
		}
	}
}