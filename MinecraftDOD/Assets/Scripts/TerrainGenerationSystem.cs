using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.GraphicsIntegration;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

namespace TechnOllieG
{
	public class TerrainGenerationSystem : SystemBase
	{
		public static TerrainGenerationSystem Instance;

		private GameData _data;
		private List<int2> _chunksToGenerate = new List<int2>();
		private List<int2> _chunksToDegenerate = new List<int2>();
		private List<int2> _currentlyGeneratedChunks = new List<int2>();
		private EntityQueryDesc _objectsWithoutCoordinateComponentDesc;
		private Transform _cameraTf;
		private List<int2> _chunksScheduledForGeneration = new List<int2>();

		protected override void OnStartRunning()
		{
			if (Instance == null)
				Instance = this;
			else
				Enabled = false;
			
			_data = GetSingleton<GameData>();

			_objectsWithoutCoordinateComponentDesc = new EntityQueryDesc
			{
				None = new ComponentType[] {typeof(ChunkCoordinate)},
				All = new ComponentType[] {typeof(Translation), ComponentType.ReadOnly<BlockState>()}
			};

			_cameraTf = Camera.main.transform;
		}

		protected override void OnUpdate()
		{
			Vector3 cameraPos = _cameraTf.position;
			DetermineChunksToGenerate(ConvertPositionToChunkCoordinate(cameraPos));
			GenerateChunks();
			CalculateExposedQuads();
			DegenerateChunks();
		}
		
		private void CalculateExposedQuads()
		{
			foreach(int2 chunk in _chunksScheduledForGeneration)
			{
				ChunkCoordinate coordinate = new ChunkCoordinate {chunkCoordinate = chunk};
				
				Entities.WithSharedComponentFilter(coordinate).ForEach(
					(ref BlockState blockState, in Translation translation) =>
					{
						byte currentBitmask = 0;
						NativeArray<float3> neighbors = new NativeArray<float3>(6, Allocator.Temp);
						{
							neighbors[0] = translation.Value.WithAdd(x: 1);
							neighbors[1] = translation.Value.WithAdd(x: -1);
							neighbors[2] = translation.Value.WithAdd(y: 1);
							neighbors[3] = translation.Value.WithAdd(y: -1);
							neighbors[4] = translation.Value.WithAdd(z: 1);
							neighbors[5] = translation.Value.WithAdd(z: -1);
						}

						for (int i = 0; i < neighbors.Length; i++)
						{
							// todo finish
						}

						neighbors.Dispose();
					}).Schedule();
			}

			_chunksScheduledForGeneration.Clear();
		}

		private Mesh MakeQuadMesh()
		{
			Vector3[] vertices = new Vector3[4]
			{
				new Vector3(0, 0, 0),
				new Vector3(1, 0, 0),
				new Vector3(0, 0, 1),
				new Vector3(1, 0, 1)
			};
			

			int[] tris = new int[6]
			{
				// lower left triangle
				0, 2, 1,
				// upper right triangle
				2, 3, 1
			};

			Vector3[] normals = new Vector3[4]
			{
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			};

			Vector2[] uv = new Vector2[4]
			{
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(0, 1),
				new Vector2(1, 1)
			};

			Mesh mesh = new Mesh {vertices = vertices, triangles = tris, normals = normals, uv = uv};
			mesh.Optimize();

			return mesh;

			// Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0f, 100f, 0f), Quaternion.identity, Vector3.one);
			// Material material = new Material(Shader.Find("Standard"));
			// material.enableInstancing = true;
			//
			// Graphics.DrawMeshInstanced(mesh, 0, material,new [] {matrix});
			// Graphics.DrawMeshInstancedProcedural
		}

		public void SetRenderDistance(int renderDistance)
		{
			_data.renderDistance = renderDistance;
		}

		private void GenerateChunks()
		{
			if (_chunksToGenerate.Count == 0)
				return;
			Camera test = Camera.main;

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

			foreach (int2 chunk in _chunksToDegenerate)
			{
				EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<BlockState>(), ComponentType.ReadOnly<ChunkCoordinate>());
				query.SetSharedComponentFilter(new ChunkCoordinate {chunkCoordinate = chunk});
				EntityManager.DestroyEntity(query);
				_currentlyGeneratedChunks.Remove(chunk);
			}
			
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
			float scale = _data.scale * (_data.worldExtents * 2 + 1);
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

					int2 zeroIndexedChunkCoordinate =
						new int2(chunkCoordinates.x + extents, chunkCoordinates.y + extents);

					int worldSizeInBlocks = (extents * 2 + 1) * chunkSize;
					float noiseX = (float) (zeroIndexedChunkCoordinate.x * chunkSize + chunkSpaceBlockCoordinate.x) / worldSizeInBlocks * scale + xOffset;
					float noiseY = (float) (zeroIndexedChunkCoordinate.y * chunkSize + chunkSpaceBlockCoordinate.y) / worldSizeInBlocks * scale + yOffset;

					int2 chunkCoord2d = chunkCoordinates * chunkSize + chunkSpaceBlockCoordinate;
					
					int3 blockPos = new int3(chunkCoord2d.x,
						(int) math.round(Mathf.PerlinNoise(noiseX, noiseY) * depth - layer), chunkCoord2d.y);

					translation.Value = blockPos;

				}).ScheduleParallel();
			
			_currentlyGeneratedChunks.Add(chunkCoordinates);
			_chunksScheduledForGeneration.Add(chunkCoordinates);
		}
		
		private void DetermineChunksToGenerate(int2 currentChunk)
		{
			int renderDistance = _data.renderDistance;
			int worldExtents = _data.worldExtents;
			List<int2> currentChunksToRender = new List<int2> {currentChunk};
			
			for (int i = 1; i <= renderDistance; i++)
			{
				int startOfCardinals = currentChunksToRender.Count;
				
				// Cardinal directions
				currentChunksToRender.Add(currentChunk.WithAdd(x: i));
				currentChunksToRender.Add(currentChunk.WithAdd(x: -i));
				currentChunksToRender.Add(currentChunk.WithAdd(y: i));
				currentChunksToRender.Add(currentChunk.WithAdd(y: -i));

				int2 cardinal1 = currentChunksToRender[startOfCardinals];
				int2 cardinal2 = currentChunksToRender[startOfCardinals + 1];

				// Along y axis on both x cardinals
				bool p = true;
				for (int j = 0; j < 2; j++)
				{
					for (int k = 1; k <= renderDistance; k++)
					{
						currentChunksToRender.Add(cardinal1 + new int2(0, p ? 1 : -1) * k);
					}
					
					for (int k = 1; k <= renderDistance; k++)
					{
						currentChunksToRender.Add(cardinal2 + new int2(0, p ? 1 : -1) * k);
					}

					p = !p;
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

				if (current.x > worldExtents || current.x < -worldExtents ||
				    current.y > worldExtents || current.y < -worldExtents)
					continue;
				
				_chunksToGenerate.Add(current);
			}
		}
		
		int2 ConvertPositionToChunkCoordinate(float3 position)
		{
			return new int2((int) math.floor(position.x / _data.chunkSize), (int) math.floor(position.z / _data.chunkSize));
		}
	}
}