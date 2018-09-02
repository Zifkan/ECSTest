using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class FiringSystem : JobComponentSystem
    {
        [Inject]
        private FiringBarrier _firingBarrier;

        private ComponentGroup _componentGroup;

      //  private static GameObject _prefab;

        protected override void OnCreateManager(int capacity)
        {
            _componentGroup = GetComponentGroup(
                ComponentType.Create<FiringComponent>(),
                ComponentType.Create<Position>(),
                ComponentType.Create<Rotation>());

            _componentGroup.SetFilterChanged(ComponentType.Create<FiringComponent>());

          
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new FiringJob
            {
                EntityCommandBuffer = _firingBarrier.CreateCommandBuffer(),
                Rotation = _componentGroup.GetComponentDataArray<Rotation>(),
                Positions = _componentGroup.GetComponentDataArray<Position>(),
                CurrentTime = Time.time
            }.Schedule(_componentGroup.CalculateLength(), 64, inputDeps);
        }

        private struct FiringJob : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent EntityCommandBuffer;
            public ComponentDataArray<Position> Positions;
            public ComponentDataArray<Rotation> Rotation;
            [ReadOnly]
            public float CurrentTime;

            public void Execute(int index)
            {
                EntityCommandBuffer.CreateEntity();
                EntityCommandBuffer.AddComponent(new BulletComponent());
                EntityCommandBuffer.AddSharedComponent(Bootstrap.BulletRenderer);
                EntityCommandBuffer.AddSharedComponent(new MoveForward());
                EntityCommandBuffer.AddComponent(new MoveSpeed{Speed = 20f});
                EntityCommandBuffer.AddComponent(Positions[index]);
                EntityCommandBuffer.AddComponent(Rotation[index]);
                EntityCommandBuffer.AddComponent(new EntityLife
                {
                    LifeTime = 2,
                    CreatedTime = CurrentTime
                });
            }
        }

   
        private class FiringBarrier: BarrierSystem
        {
            
        }
    }
}