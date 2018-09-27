using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Systems
{
    public class CleanUpEntitySystem : JobComponentSystem
    {
        [Inject]
        private Data _data;

        [Inject]
        private CleanUpEntityBarrier _bulletRayBarrier;
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new CleanUpEnityJob()
            {
                EntityCommandBuffer = _bulletRayBarrier.CreateCommandBuffer(),
                EntityArray = _data.Entities
            }.Schedule(_data.Length, 64, inputDeps);
        }

        private struct CleanUpEnityJob : IJobParallelFor
        {
            [ReadOnly]
            public EntityCommandBuffer EntityCommandBuffer;

            [ReadOnly]
            public EntityArray EntityArray;

            public void Execute(int index)
            {
                EntityCommandBuffer.DestroyEntity(EntityArray[index]);
            }
        }


        private struct Data
        {
            public readonly int Length;
            public EntityArray Entities;
            public ComponentDataArray<DestroyEntityComponent> FiringComponents;
        }
        private class CleanUpEntityBarrier : BarrierSystem
        {
        }
    }
}