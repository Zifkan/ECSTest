using UnityEngine;

namespace SriptableObjects
{
    //[CreateAssetMenu(fileName = "UnitViewSettings", menuName = "UnitView/UnitViewSettings", order = 1)]
    public class UnitViewSettings : ScriptableObject
    {
        public GameObject MeleePrefab;
        public GameObject SkeletonPrefab;

        private static UnitViewSettings _instance;

        public static UnitViewSettings Instance => _instance ?? (_instance = Resources.Load<UnitViewSettings>("UnitViewSettings"));
    }
}