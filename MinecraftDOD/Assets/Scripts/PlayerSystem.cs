using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

using RaycastHit = Unity.Physics.RaycastHit;

namespace TechnOllieG
{
	public class PlayerSystem : SystemBase
	{
		private Transform _camTf;
		private CameraController _camController;
		private double _lastJump;
		private BuildPhysicsWorld _buildPhysicsWorldSystem;
		private StepPhysicsWorld _stepPhysicsWorldSystem;
 
		protected override void OnCreate()
		{
			_buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
			_stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
		}

		protected override void OnStartRunning()
		{
			_camTf = Camera.main.transform;
			_camController = _camTf.GetComponent<CameraController>();
		}

		protected override void OnUpdate()
		{
			if (PauseMenu.isPaused)
				return;
			
			if (_camController.useCameraMovement)
				return;

			float forwardInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
			float rightInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
			bool jumpInput = Input.GetKey(KeyCode.Space);
			double time = Time.ElapsedTime;

			Entities.ForEach((ref PhysicsVelocity velocity, ref Rotation rotation, in LocalToWorld localToWorld, in PlayerComponent player) =>
			{
				_camTf.position = localToWorld.Position + math.up() * 0.5f;

				rotation.Value = Quaternion.AngleAxis(_camController.accumulatedMouseDelta.x, Vector3.up);

				float3 vel = velocity.Linear;

				float3 acceleration = float3.zero;
				if (forwardInput > 0.5f || rightInput > 0.5f || forwardInput < -0.5f || rightInput < -0.5f)
				{
					acceleration = math.normalize(forwardInput * localToWorld.Forward + rightInput * localToWorld.Right) * player.accelerationSpeed;
				}

				vel += (acceleration - new float3(vel.x, 0f, vel.z) * player.friction) * Time.DeltaTime;
				vel -=  math.up() * (Time.DeltaTime * player.gravity);

				if(jumpInput && IsGrounded(localToWorld.Position, 1f) && time - _lastJump > player.jumpCooldown)
				{
					_lastJump = time;
					vel += localToWorld.Up * player.jumpImpulse;
				}
				
				velocity.Linear = vel;
			}).WithoutBurst().Run();
		}

		private bool IsGrounded(float3 playerPos, float extents)
		{
			return Raycast(playerPos, playerPos + math.down() * (extents + 0.1f));
		}
		
		public bool Raycast(float3 from, float3 to, out RaycastHit hit)
		{
			var collisionWorld = _buildPhysicsWorldSystem.PhysicsWorld.CollisionWorld;
			
			RaycastInput input = new RaycastInput()
			{
				Start = from,
				End = to,
				Filter = new CollisionFilter()
				{
					BelongsTo = 1 << 0,
					CollidesWith = 1 << 1,
					GroupIndex = 0
				}
			};
			
			bool blockingHit = collisionWorld.CastRay(input, out hit);
			
			return blockingHit;
		}

		public bool Raycast(float3 from, float3 to)
		{
			RaycastHit hit;
			return Raycast(from, to, out hit);
		}
	}
}