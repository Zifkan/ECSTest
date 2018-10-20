using UnityEngine;
using UnityEngine.Rendering;

namespace Test
{
    public class TestSpawner : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        [SerializeField] private int _count;

        private ComputeBuffer _positionBuffer;
        private ComputeBuffer _argsBuffer;
        private uint[] _args = new uint[5] {0, 0, 0, 0, 0};

        void Start()
        {
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateBuffers();
        }

        void UpdateBuffers()
        {
            var mesh = _meshRenderer.sharedMesh;
            var material = _meshRenderer.material;

            _positionBuffer = new ComputeBuffer(_count, 16);
            Vector4[] positions = new Vector4[_count];
            for (int i = 0; i < _count; i++)
            {
                float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
                float distance = Random.Range(20.0f, 100.0f);
                float height = Random.Range(-2.0f, 2.0f);
                float size = Random.Range(0.05f, 0.25f);
                positions[i] = new Vector4(Mathf.Sin(angle) * distance, height, Mathf.Cos(angle) * distance, size);
            }
            _positionBuffer.SetData(positions);
            material.SetBuffer("positionBuffer", _positionBuffer);



        

            _args[0] = mesh.GetIndexCount(0);
            _args[1] = (uint)_count;
            _argsBuffer.SetData(_args);

            Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, 1000000 * Vector3.one), _argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.Off, true);

        }
    }
}
