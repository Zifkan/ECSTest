using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct Collision: IComponentData
    {
        public UnityEngine.Collision Source;
        public UnityEngine.Collision Other;
    }

    public class CollisionComponent : ComponentDataWrapper<Collision>
    {
       
    }
}