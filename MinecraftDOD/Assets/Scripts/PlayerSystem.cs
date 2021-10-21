using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace TechnOllieG
{
	public class PlayerSystem : SystemBase
	{
		private float3 vel = float3.zero;
		
		protected override void OnUpdate()
		{
			Entities.ForEach((ref PhysicsVelocity velocity, in LocalToWorld localToWorld, in PlayerComponent player) =>
			{
				float forwardInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
				float rightInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
				bool jumpInput = Input.GetKey(KeyCode.Space);

				if (forwardInput > 0.5f || rightInput > 0.5f || forwardInput < -0.5f || rightInput < -0.5f)
				{
					float3 acceleration = math.normalize(forwardInput * localToWorld.Forward + rightInput * localToWorld.Right) * player.accelerationSpeed;
					vel += (acceleration - vel * player.friction) * Time.DeltaTime;
				}
				
				vel -=  localToWorld.Up * (Time.DeltaTime * player.gravity);
				velocity.Linear = vel;

				// if(jumpInput && IsGrounded())
				// {
				// 	velocity.Linear += localToWorld.Up * player.jumpImpulse;
				// }
			}).WithoutBurst().Run();

			NativeArray<Entity> players = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>()).ToEntityArray(Allocator.Temp);
			Rotation playerRotation = EntityManager.GetComponentData<Rotation>(players[0]);

			Entities.ForEach((ref Rotation rotation, in CameraComponent camera) =>
			{
				rotation.Value = math.mul(rotation.Value, playerRotation.Value);
			}).WithoutBurst().Run();
		}
	}
}