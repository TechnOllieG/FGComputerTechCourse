using Unity.Entities;
using UnityEngine;

namespace TechnOllieG
{
	public enum BlockID
	{
		Air,
		Grass,
		Dirt,
		Stone,
		Wood,
		Leaves
	}

	[GenerateAuthoringComponent]
	public struct BlockState : IComponentData
	{
		public BlockID id;

		// Bitmask specifying which sides of the block are exposed, 00{-z}{z}{-y}{y}{-x}{x}
		[HideInInspector] public byte exposedSides;
	}
}