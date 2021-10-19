using Unity.Entities;
using Unity.Mathematics;

namespace TechnOllieG
{
	public struct ChunkCoordinate : ISharedComponentData
	{
		public int2 chunkCoordinate;
	}
}