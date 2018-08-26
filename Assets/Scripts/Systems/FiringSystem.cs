using Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class FiringSystem : JobComponentSystem
    {
        [Inject]
        private FiringBarrier _firingBarrier;

        private ComponentGroup _componentGroup;

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

            }.Schedule(_componentGroup.CalculateLength(), 64, inputDeps);
        }

        private struct FiringJob : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent EntityCommandBuffer;
            public ComponentDataArray<Position> Positions;
            public ComponentDataArray<Rotation> Rotation;
            public void Execute(int index)
            {
                EntityCommandBuffer.CreateEntity();
                EntityCommandBuffer.AddSharedComponent(Bootstrap.BulletRenderer);
              //  EntityCommandBuffer.AddComponent(new TransformMatrix());
                EntityCommandBuffer.AddSharedComponent(new MoveForward());
                EntityCommandBuffer.AddComponent(new MoveSpeed{Speed = 6f});
                EntityCommandBuffer.AddComponent(Positions[index]);
                EntityCommandBuffer.AddComponent(Rotation[index]);
            }
        }


        private class FiringBarrier: BarrierSystem
        {
            
        }
    }
}