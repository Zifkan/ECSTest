using System.Threading;
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
    unsafe class BulletsRayCastsSystem : JobComponentSystem
    {
        private ComponentGroup _bulletsGroup;

        [Inject]
        private BulletRayBarrier _bulletRayBarrier;

        protected override void OnCreateManager(int capacity)
        {
            _bulletsGroup = GetComponentGroup(typeof(BulletComponent),typeof(Position),
               typeof(Rotation),ComponentType.Subtractive(typeof(DestroyEntityComponent)));

         //   _bulletsGroup.SetFilterChanged(ComponentType.Create<BulletComponent>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var positions = _bulletsGroup.GetComponentDataArray<Position>();
            var rotations = _bulletsGroup.GetComponentDataArray<Rotation>();


            if (positions.Length == 0) return inputDeps;

            inputDeps.Complete();

            

            for (int i = 0; i < positions.Length; i++)
            {
                var commands = new NativeArray<RaycastCommand>(_bulletsGroup.CalculateLength(), Allocator.TempJob);
                var hits = new NativeArray<RaycastHit>(_bulletsGroup.CalculateLength(), Allocator.TempJob);


                // 1: Set-up jobs
                var setupJob = new SetupJob
                {
                    Commands = commands,
                    Position = positions[i].Value,
                    Direction = rotations[i].Value,
                };
                var deps = setupJob.Schedule(_bulletsGroup.CalculateLength(), 16, inputDeps);

                // 2: Raycast jobs
                deps = RaycastCommand.ScheduleBatch(commands, hits, 16, deps);

                // 3: Transfer jobs
                var transferJob = new TransferJob
                {
                    EntityCommandBuffer = _bulletRayBarrier.CreateCommandBuffer(),
                    Entities = _bulletsGroup.GetEntityArray(),
                    RaycastCommands = commands,
                    RaycastHits = hits,
                    
                };
                inputDeps = transferJob.Schedule(_bulletsGroup.CalculateLength(), 64, deps);
            }

          return inputDeps;

    
        }

       // [Unity.Burst.BurstCompile]
        struct SetupJob : IJobParallelFor
        {
            // Output
            public NativeArray<RaycastCommand> Commands;

            // Common parameters
            public float3 Position;
            public quaternion Direction;

            public void Execute(int i)
            {
                Commands[i] = new RaycastCommand(Position, math.forward(Direction).xyz, 2);
            }
        }

       // [Unity.Burst.BurstCompile]
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
                 /*var from = (float3)RaycastCommands[index].from;
                var dist = RaycastHits[index].distance;*/
                if (RaycastHits[index].point == Vector3.zero) return;
                Debug.Log(Entities[index]);
                EntityCommandBuffer.AddComponent(Entities[index],new DestroyEntityComponent());
            }
        }

        private class BulletRayBarrier : BarrierSystem
        {

        }
    }
}