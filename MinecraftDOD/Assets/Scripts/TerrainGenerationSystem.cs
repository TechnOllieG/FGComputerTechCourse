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

		protected override void OnStartRunning()
		{
			_data = GetSingleton<GameData>();
			GenerateChunk(new int2(0, 0));
		}

		protected override void OnUpdate()
		{
			
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
			
			Entities.ForEach((int entityInQueryIndex, ref Translation translation, in BlockState state) =>
			{
				int layer = entityInQueryIndex / chunkArea;
				int chunkIndex = entityInQueryIndex % chunkArea;
				int2 chunkSpaceBlockCoordinate = new int2(chunkIndex % chunkSize, chunkIndex / chunkSize);
				
				int halfChunkSize = chunkSize / 2;
				float2 offsetFromCenter = math.remap(float2.zero, new float2(chunkSize - 1),
					new float2(-halfChunkSize + 0.5f), new float2(halfChunkSize - 0.5f),
					chunkSpaceBlockCoordinate);
				
				int2 zeroIndexedChunkCoordinate = (int2) math.remap(new float2(-extents), new float2(extents), float2.zero,
					new float2(extents * 2), chunkCoordinates);

				int worldSizeInBlocks = (extents * 2 + 1) * chunkSize;
				float noiseX = (float) (zeroIndexedChunkCoordinate.x * chunkSize + chunkSpaceBlockCoordinate.x) / worldSizeInBlocks * scale + xOffset;
				float noiseY = (float) (zeroIndexedChunkCoordinate.y * chunkSize + chunkSpaceBlockCoordinate.y) / worldSizeInBlocks * scale + yOffset;

				float2 chunkCoord2d = chunkCoordinates * chunkSize + offsetFromCenter;

				float x = chunkCoord2d.x;
				float y = Mathf.PerlinNoise(noiseX, noiseY) * depth - layer;
				float z = chunkCoord2d.y;

				translation.Value.x = x;
				translation.Value.y = math.round(y);
				translation.Value.z = z;
			}).ScheduleParallel();
		}
	}
}