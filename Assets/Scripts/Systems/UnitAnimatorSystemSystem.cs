using System;
using System.Collections.Generic;
using Components;
using Data;
using SriptableObjects;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(BulletsRayCastsSystem))]
    public class UnitAnimatorSystemSystem : JobComponentSystem
    {
        private NativeArray<AnimationClipDataBaked> _animationClipData;
        private Dictionary<UnitType, Data.DataPerUnitType> _perUnitTypeDataHolder;

        protected override void OnCreateManager()
        {
           // Initialize();
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }

        private struct ShaderValuesChange : IJobParallelFor
        {


            public void Execute(int index)
            {

            }
        }

        private void Initialize()
        {
            _animationClipData = new NativeArray<AnimationClipDataBaked>(100, Allocator.Persistent);

            _perUnitTypeDataHolder = new Dictionary<UnitType, Data.DataPerUnitType>();

            InstantiatePerUnitTypeData(UnitType.Melee);
        }

        private void InstantiatePerUnitTypeData(UnitType type)
        {
            var gameObject = type == UnitType.Melee? UnitViewSettings.Instance.MeleePrefab: UnitViewSettings.Instance.SkeletonPrefab;
            var renderingData = gameObject.GetComponentInChildren<RenderingDataWrapper>().Value;
            SkinnedMeshRenderer renderer = renderingData.SkinnedMeshRenderer;
            Material material = renderingData.Material;
            LodData lodData = renderingData.LodData;

            var animationComponent = gameObject.GetComponent<Animation>();

            var dataPerUnitType = new Data.DataPerUnitType
            {
                BakedData = KeyframeTextureBaker.BakeClips(renderer, animationComponent, lodData),
                Material = material,
            };

            dataPerUnitType.Drawer = new InstancedSkinningDrawer(dataPerUnitType, dataPerUnitType.BakedData.NewMesh);
            dataPerUnitType.Lod1Drawer = new InstancedSkinningDrawer(dataPerUnitType, dataPerUnitType.BakedData.Lods.Lod1Mesh);
            dataPerUnitType.Lod2Drawer = new InstancedSkinningDrawer(dataPerUnitType, dataPerUnitType.BakedData.Lods.Lod2Mesh);
            dataPerUnitType.Lod3Drawer = new InstancedSkinningDrawer(dataPerUnitType, dataPerUnitType.BakedData.Lods.Lod3Mesh);


            _perUnitTypeDataHolder.Add(type, dataPerUnitType);
           // TransferAnimationData(type);
        }

        private struct AnimationClipDataBaked
        {
            public float TextureOffset;
            public float TextureRange;
            public float OnePixelOffset;
            public int TextureWidth;

            public float AnimationLength;
          //  public bool Looping;
        }

        private class DataPerUnitType
        {
            public KeyframeTextureBaker.BakedData BakedData;

            public InstancedSkinningDrawer Drawer;
            public InstancedSkinningDrawer Lod1Drawer;
            public InstancedSkinningDrawer Lod2Drawer;
            public InstancedSkinningDrawer Lod3Drawer;
            public NativeArray<IntPtr> BufferPointers;

            public Material Material;
            public int Count;

            public void Dispose()
            {
                Drawer?.Dispose();
                Lod1Drawer?.Dispose();
                Lod2Drawer?.Dispose();
                Lod3Drawer?.Dispose();

                if (BufferPointers.IsCreated) BufferPointers.Dispose();
            }
        }
    }

}
