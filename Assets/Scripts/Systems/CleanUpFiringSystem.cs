using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Systems
{
    public class CleanUpFiringSystem : JobComponentSystem
    {
        [Inject]private Data _data;
        [Inject] private CleanupFiringBarrier _cleanupFiringBarrier;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new CleanUpFiringJob
            {
                EntityCommandBuffer = _cleanupFiringBarrier.CreateCommandBuffer(),
                Entities = _data.Entities,
                FiringComponents = _data.FiringComponents,
                CurrentTime = Time.time
            }.Schedule(_data.Length, 64, inputDeps);
        }

        private struct CleanUpFiringJob : IJobParallelFor
        {
            [ReadOnly]
            public EntityArray Entities;
            public EntityCommandBuffer.Concurrent EntityCommandBuffer;
            public ComponentDataArray<FiringComponent> FiringComponents;
            public float CurrentTime;

            public void Execute(int index)
            {
                if (CurrentTime - FiringComponents[index].FiredAt < 0.5f) return;
                    EntityCommandBuffer.RemoveComponent<FiringComponent>(Entities[index]);
            }
        }

        private struct Data
        {
            public readonly int Length;
            public EntityArray Entities;
            public ComponentDataArray<FiringComponent> FiringComponents;
        }
    }

    public class CleanupFiringBarrier:BarrierSystem
    {
    }
}