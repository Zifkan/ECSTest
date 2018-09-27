using System;
using Unity.Collections;
using UnityEngine;

namespace Data
{
    public class DataPerUnitType
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