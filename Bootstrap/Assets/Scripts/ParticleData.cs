using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ParticleData : ISharedComponentData
{
	public Mesh particleMesh;
	public Material particleMaterial;
	public float spinRate;
	public float upwardSpeed;
}