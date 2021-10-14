using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // There is a single Settings component in the scene.
        var settings = GetSingleton<Settings>();

        // Use the prefab from Settings to create a given amount of instances.
        var instances = new NativeArray<Entity>(settings.Count, Allocator.Temp);
        EntityManager.Instantiate(settings.Prefab, instances);

        // Loop of the instances to place them along the X axis.
        for (int i = 0; i < instances.Length; i++)
        {
            EntityManager.SetComponentData(instances[i], new Translation {Value = new float3(i * 1.1f, 0, 0)});
        }

        // Native containers require explicit cleanup. There are special rules around
        // Allocator.Temp that do not require cleanup, but calling Dispose is never wrong.
        instances.Dispose();

        // Disabling the system so it only runs once, this one is for initialization only.
        Enabled = false;
    }
}