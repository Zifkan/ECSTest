using System;
using Unity.Entities;

namespace Components
{
    [Serializable]
    public struct EntityLife : IComponentData
    {
        public float CreatedTime;
        public float LifeTime;
    }

    public class EntityLifeComponent : ComponentDataWrapper<EntityLife>
    {
    }
}
