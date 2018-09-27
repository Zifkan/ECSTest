using System;
using Unity.Entities;

namespace Components
{
    [Serializable]
    public struct TextureAnimatorData : IComponentData
    {
        public float AnimationNormalizedTime;

        public int CurrentAnimationId;
        public int NewAnimationId;

        public int UnitType;

        public float AnimationSpeedVariation;
    }

    public class TextureAnimatorDataComponent : ComponentDataWrapper<TextureAnimatorData> { }
}