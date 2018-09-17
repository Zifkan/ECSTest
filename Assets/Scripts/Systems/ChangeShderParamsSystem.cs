using System;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(BulletsRayCastsSystem))]
    public class ChangeShaderParamsSystem : JobComponentSystem
    {
        private MaterialPropertyBlock _materialPropertyBlock;
        private ComponentGroup _bulletsGroup;


        protected override void OnCreateManager(int capacity)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();

            _bulletsGroup = GetComponentGroup(typeof(BulletComponent), typeof(MeshRenderer), ComponentType.Subtractive(typeof(DestroyEntityComponent)));
            Debug.Log(_bulletsGroup.CalculateLength());
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Debug.Log(_bulletsGroup.CalculateLength());
            for (int i = 0; i < _bulletsGroup.CalculateLength(); i++)
            {
              // _bulletsGroup.GetSharedComponentDataArray<MeshInstanceRenderer>()[i].material
            }


            return inputDeps;
        }

        private struct ShaderValuesChange : IJobParallelFor
        {
            

            public void Execute(int index)
            {
              
            }
        }
    }
}