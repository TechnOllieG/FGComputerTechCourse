using Unity.Entities;

namespace TechnOllieG
{
	[GenerateAuthoringComponent]
	public struct PlayerComponent : IComponentData
	{
		public float accelerationSpeed;
		public float friction;
		public float jumpImpulse;
		public float gravity;
	}
}