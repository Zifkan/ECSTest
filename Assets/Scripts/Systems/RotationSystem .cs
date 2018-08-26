using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public class RotationSystem : ComponentSystem
    {
        [Inject]
        private Data _data;

        protected override void OnUpdate()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                var rotation = _data.RotationComponents[i].Value;
               _data.Rigidbodies[i].MoveRotation(rotation);
            }
        }

        private struct Data
        {
            public readonly int Length;
            public ComponentArray<RotationComponent> RotationComponents;
            public ComponentArray<Rigidbody> Rigidbodies;
        }
    }
}