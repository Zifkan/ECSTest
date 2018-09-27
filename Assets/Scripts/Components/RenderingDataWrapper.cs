using System;
using Data;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    public class RenderingDataWrapper : SharedComponentDataWrapper<RenderingData>
    {

    }

    [Serializable]
    public struct RenderingData : ISharedComponentData
    {
        public UnitType UnitType;
        //public GameObject BakingPrefab;
        public Material Material;
        public SkinnedMeshRenderer SkinnedMeshRenderer;
        public LodData LodData;
    }
}