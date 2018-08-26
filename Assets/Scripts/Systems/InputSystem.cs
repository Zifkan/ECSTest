using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public class InputSystem : ComponentSystem
    {
        [Inject]
        private Data _data;
       
        protected override void OnUpdate()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            for (int i = 0; i < _data.Length; i++)
            {
                
                _data.InputComponents[i].Horizontal = horizontal;
                _data.InputComponents[i].Vertical = vertical;
            }
        }

        private struct Data
        {
            public readonly int Length;
            public ComponentArray<InputComponent> InputComponents;
        }
    }
}