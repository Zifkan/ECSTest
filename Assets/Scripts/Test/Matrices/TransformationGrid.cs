using System.Collections.Generic;
using UnityEngine;

namespace Test.Matrices
{
    public class TransformationGrid : MonoBehaviour
    {
        public Transform Prefab;

        public int GridResolution = 10;

        private Transform[] _grid;

        private List<Transformation> _transformations;

      
        void Awake()
        {
            _transformations = new List<Transformation>();

            _grid = new Transform[GridResolution * GridResolution * GridResolution];
            for (int i = 0, z = 0; z < GridResolution; z++)
            {
                for (int y = 0; y < GridResolution; y++)
                {
                    for (int x = 0; x < GridResolution; x++, i++)
                    {
                        _grid[i] = CreateGridPoint(x, y, z);
                    }
                }
            }
        }

        void Update()
        {
            GetComponents<Transformation>(_transformations);
            for (int i = 0, z = 0; z < GridResolution; z++)
            {
                for (int y = 0; y < GridResolution; y++)
                {
                    for (int x = 0; x < GridResolution; x++, i++)
                    {
                        _grid[i].localPosition = TransformPoint(x, y, z);
                    }
                }
            }
        }

        Vector3 TransformPoint(int x, int y, int z)
        {
            Vector3 coordinates = GetCoordinates(x, y, z);
            for (int i = 0; i < _transformations.Count; i++)
            {
                coordinates = _transformations[i].Apply(coordinates);
            }
            return coordinates;
        }

        Transform CreateGridPoint(int x, int y, int z)
        {
            Transform point = Instantiate<Transform>(Prefab);
            point.localPosition = GetCoordinates(x, y, z);
            point.GetComponent<MeshRenderer>().material.color = new Color(
                (float)x / GridResolution,
                (float)y / GridResolution,
                (float)z / GridResolution
            );
            return point;
        }

        Vector3 GetCoordinates(int x, int y, int z)
        {
            return new Vector3(
                x - (GridResolution - 1) * 0.5f,
                y - (GridResolution - 1) * 0.5f,
                z - (GridResolution - 1) * 0.5f
            );
        }
    }
}