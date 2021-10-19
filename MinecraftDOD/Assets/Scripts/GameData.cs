using Unity.Entities;
using UnityEngine;

namespace TechnOllieG
{
	[GenerateAuthoringComponent]
	public struct GameData : IComponentData
	{
		public Entity grass;
		public Entity dirt;
		public Entity stone;
		public Entity wood;
		public Entity leaves;
		
		[Tooltip("World size in chunks")]
		public int worldExtents;
		
		[Tooltip("Chunk size in blocks")]
		public int chunkSize;
		public int layersOfDirt;
		public int layersOfStone;
		[Tooltip("The render distance in extents, 0 will only render the current chunk you're in, 1 will render a 3x3 around you etc.")]
		public int renderDistance;

		public float scale;
		public float depth;
		public float xOffset;
		public float yOffset;
	}
}