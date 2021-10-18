using Unity.Entities;

namespace TechnOllieG
{
	public enum BlockID
	{
		None = -1,
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
	}
}