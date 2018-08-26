using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public class PlayerRotationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var mousePosition = Input.mousePosition;
            var cameraRay = Camera.main.ScreenPointToRay(mousePosition);
            var layermask = LayerMask.GetMask("Floor");

            if (Physics.Raycast(cameraRay, out var hit, 100, layermask))
            {
                foreach (var entity in GetEntities<Filter>())
                {
                    var forward = hit.point - entity.Transform.position;
                    var rotation = Quaternion.LookRotation(forward);

                    entity.RotationComponent.Value = new Quaternion(0,rotation.y, 0,rotation.w).normalized;
                }
            }
        }

        private struct Filter
        {
            public Transform Transform;
            public RotationComponent RotationComponent;
        }

        
    }
}