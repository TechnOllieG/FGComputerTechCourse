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

		public float scale;
		public float depth;
		public float xOffset;
		public float yOffset;
	}
}