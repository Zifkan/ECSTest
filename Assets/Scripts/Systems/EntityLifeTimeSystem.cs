using Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

namespace Systems
{
    public class EntityLifeTimeSystem: JobComponentSystem
    {
        private ComponentGroup _entityGroup;

        [Inject]
        private EntityLifeTimeBarrier _lifeTimeBarrier;

        protected override void OnCreateManager(int capacity)
        {
            _entityGroup = GetComponentGroup(typeof(EntityLife),ComponentType.Subtractive(typeof(DestroyEntityComponent)));
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new EntityLifeTimeJob()
            {
                CommandBuffer = _lifeTimeBarrier.CreateCommandBuffer(),
                EntityArray = _entityGroup.GetEntityArray(),
                EntityLifeArray = _entityGroup.GetComponentDataArray<EntityLife>(),
                CurrentTime = Time.time
            }.Schedule(_entityGroup.CalculateLength(),64, inputDeps);
        }

        struct EntityLifeTimeJob : IJobParallelFor
        {
            [ReadOnly]
            public EntityArray EntityArray;

            public EntityCommandBuffer.Concurrent CommandBuffer;

            [ReadOnly]
            public ComponentDataArray<EntityLife> EntityLifeArray;

            [ReadOnly]
            public float CurrentTime;
            public void Execute(int i)
            {
                if (CurrentTime - EntityLifeArray[i].CreatedTime >= EntityLifeArray[i].LifeTime)
                {
                    CommandBuffer.AddComponent(EntityArray[i], new DestroyEntityComponent());
                }
            }
        }

        private class EntityLifeTimeBarrier:BarrierSystem
        {
        }
    }
}