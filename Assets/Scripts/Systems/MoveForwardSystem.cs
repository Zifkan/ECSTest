using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class MoveForwardSystem : JobComponentSystem
    {
        private ComponentGroup _moveForwardGroup;

        protected override void OnCreateManager()
        {
            _moveForwardGroup = GetComponentGroup(
                ComponentType.ReadOnly(typeof(MoveForward)),
                ComponentType.ReadOnly(typeof(MoveSpeed)),
                ComponentType.ReadOnly(typeof(Rotation)),
                typeof(Position));

        //    _moveForwardGroup.SetFilterChanged(ComponentType.Create<MoveForward>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new MoveForwardRotation
            {
                positions = _moveForwardGroup.GetComponentDataArray<Position>(),
                rotations = _moveForwardGroup.GetComponentDataArray<Rotation>(),
                moveSpeeds = _moveForwardGroup.GetComponentDataArray<MoveSpeed>(),
                dt = Time.deltaTime
            }.Schedule(_moveForwardGroup.CalculateLength(), 64, inputDeps);
        }

        //[BurstCompile]
        private struct MoveForwardRotation : IJobParallelFor
        {
            public ComponentDataArray<Position> positions;
            [ReadOnly] public ComponentDataArray<Rotation> rotations;
            [ReadOnly] public ComponentDataArray<MoveSpeed> moveSpeeds;
            public float dt;

            public void Execute(int i)
            {

                var pos = positions[i].Value + dt * moveSpeeds[i].Speed * math.forward(rotations[i].Value);
                positions[i] = new Position
                {
                    Value = pos
                };
            }
        }
    }
}