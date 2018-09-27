using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Systems
{
    public class PlayerShootingSystem : JobComponentSystem
    {
        [Inject]
        private Data _data;

        [Inject]
        private PlayerShootingBarrier _playerShootingBarrier;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetButton("Fire1"))
            {
                return new PlayerShootingJob
                {
                    EntityArray = _data.EntityArray,
                    EntityCommandBuffer = _playerShootingBarrier.CreateCommandBuffer(),
                    CurrentTime = Time.time
                }.Schedule(_data.Length, 64, inputDeps);
            }
            return base.OnUpdate(inputDeps);
        }

        private struct Data
        {
            public readonly int Length;
            public EntityArray EntityArray;
            public ComponentDataArray<Weapon> WeapomComponents;
            public SubtractiveComponent<FiringComponent> Firings;
        }

        
        private struct PlayerShootingJob : IJobParallelFor
        {
            [ReadOnly]
            public EntityArray EntityArray;

            [ReadOnly]
            public EntityCommandBuffer EntityCommandBuffer;
            public float CurrentTime;

            public void Execute(int index)
            {
                EntityCommandBuffer.AddComponent(EntityArray[index],new FiringComponent
                {
                    FiredAt = CurrentTime
                });
            }
        }
    }

    public class PlayerShootingBarrier : BarrierSystem
    {

    }
}