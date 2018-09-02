using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(FiringSystem))]
    class BulletsRayCastsSystem : JobComponentSystem
    {
        private ComponentGroup _bulletsGroup;

        [Inject]
        private BulletRayBarrier _bulletRayBarrier;

        protected override void OnCreateManager(int capacity)
        {
            _bulletsGroup = GetComponentGroup(typeof(BulletComponent),typeof(Position),
               typeof(Rotation),ComponentType.Subtractive(typeof(DestroyEntityComponent)));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var positions = _bulletsGroup.GetComponentDataArray<Position>();
            var rotations = _bulletsGroup.GetComponentDataArray<Rotation>();

           
            if (positions.Length == 0) return inputDeps;

           
            var raycastCommands = new NativeArray<RaycastCommand>(_bulletsGroup.CalculateLength(), Allocator.TempJob);
            var raycastHits = new NativeArray<RaycastHit>(_bulletsGroup.CalculateLength(), Allocator.TempJob);

            var setupRaycastsJob = new RaycastHitCommands()
            {
                Positions = positions,
                Directions = rotations,
                Raycasts = raycastCommands,
            };

            var setupDependency = setupRaycastsJob.Schedule(_bulletsGroup.CalculateLength(), 32, inputDeps);

            var deps = RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, 32, setupDependency);

            deps.Complete();

            var transferJob = new TransferJob
            {
                EntityCommandBuffer = _bulletRayBarrier.CreateCommandBuffer(),
                Entities = _bulletsGroup.GetEntityArray(),
                RaycastCommands = raycastCommands,
                RaycastHits = raycastHits,
            };

            inputDeps = transferJob.Schedule(_bulletsGroup.CalculateLength(), 64, deps);

            return inputDeps;
        }

        struct TransferJob : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent EntityCommandBuffer;

            [ReadOnly]
            public EntityArray Entities;

            [DeallocateOnJobCompletion]
            [ReadOnly]
            public NativeArray<RaycastCommand> RaycastCommands;

            [DeallocateOnJobCompletion]
            [ReadOnly]
            public NativeArray<RaycastHit> RaycastHits;
            
            public void Execute(int index)
            {
                if (RaycastHits[index].point != Vector3.zero)
                {
                    EntityCommandBuffer.AddComponent(Entities[index], new DestroyEntityComponent());
                }
            }
        }

        struct RaycastHitCommands : IJobParallelFor
        {
            public NativeArray<RaycastCommand> Raycasts;
            [ReadOnly]
            public ComponentDataArray<Position> Positions;
            [ReadOnly]
            public ComponentDataArray<Rotation> Directions;
            
            public void Execute(int i)
            {
                Raycasts[i] = new RaycastCommand(Positions[i].Value, math.forward(Directions[i].Value), 2);
            }
        }

        private class BulletRayBarrier : BarrierSystem
        {

        }
    }
}