using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace Data
{
    public class InstancedSkinningDrawer : IDisposable
    {
        private const int PreallocatedBufferSize = 32 * 1024;

        private readonly ComputeBuffer _argsBuffer;

        private readonly uint[] _indirectArgs =  { 0, 0, 0, 0, 0 };

        private readonly ComputeBuffer _textureCoordinatesBuffer;
        private readonly ComputeBuffer _objectRotationsBuffer;
        private readonly ComputeBuffer _objectPositionsBuffer;

        public NativeArray<float3> TextureCoordinates;
        public NativeArray<float4> ObjectPositions;
        public NativeArray<quaternion> ObjectRotations;
        
        public int UnitToDrawCount => ObjectPositions.Length;


        private readonly Material _material;
        private readonly Mesh _mesh;
        private readonly DataPerUnitType _data;


        public InstancedSkinningDrawer(DataPerUnitType data, Mesh meshToDraw)
        {
            _data = data;
            _mesh = meshToDraw;
            _material = new Material(data.Material);

            _argsBuffer = new ComputeBuffer(1, _indirectArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            _indirectArgs[0] = _mesh.GetIndexCount(0);
            _indirectArgs[1] = (uint)0;
            _argsBuffer.SetData(_indirectArgs);

            _objectRotationsBuffer = new ComputeBuffer(PreallocatedBufferSize, 16);
            _objectPositionsBuffer = new ComputeBuffer(PreallocatedBufferSize, 16);
            _textureCoordinatesBuffer = new ComputeBuffer(PreallocatedBufferSize, 12);


            TextureCoordinates = new NativeList<float3>(PreallocatedBufferSize, Allocator.Persistent);
            ObjectPositions = new NativeList<float4>(PreallocatedBufferSize, Allocator.Persistent);
            ObjectRotations = new NativeList<quaternion>(PreallocatedBufferSize, Allocator.Persistent);

            
            _material.SetBuffer("textureCoordinatesBuffer", _textureCoordinatesBuffer);
            _material.SetBuffer("objectPositionsBuffer", _objectPositionsBuffer);
            _material.SetBuffer("objectRotationsBuffer", _objectRotationsBuffer);
            _material.SetTexture("_AnimationTexture0", data.BakedData.Texture0);
            _material.SetTexture("_AnimationTexture1", data.BakedData.Texture1);
            _material.SetTexture("_AnimationTexture2", data.BakedData.Texture2);

        }

        public void Dispose()
        {
            _argsBuffer?.Dispose();
            _objectPositionsBuffer?.Dispose();
            if (ObjectPositions.IsCreated) ObjectPositions.Dispose();

            _objectRotationsBuffer?.Dispose();
            if (ObjectRotations.IsCreated) ObjectRotations.Dispose();

            _textureCoordinatesBuffer?.Dispose();
            if (TextureCoordinates.IsCreated) TextureCoordinates.Dispose();
        }

        public void Draw()
        {
            if (_objectRotationsBuffer == null || _data.Count == 0) return;

            int count = UnitToDrawCount;
            if (count == 0) return;

            Profiler.BeginSample("Modify compute buffers");

            Profiler.BeginSample("Shader set data");


            _objectPositionsBuffer.SetData(ObjectPositions, 0, 0, count);
            _objectRotationsBuffer.SetData(ObjectRotations, 0, 0, count);
            _textureCoordinatesBuffer.SetData(TextureCoordinates, 0, 0, count);


            _material.SetBuffer("textureCoordinatesBuffer", _textureCoordinatesBuffer);
            _material.SetBuffer("objectPositionsBuffer", _objectPositionsBuffer);
            _material.SetBuffer("objectRotationsBuffer", _objectRotationsBuffer);
            _material.SetTexture("_AnimationTexture0", _data.BakedData.Texture0);
            _material.SetTexture("_AnimationTexture1", _data.BakedData.Texture1);
            _material.SetTexture("_AnimationTexture2", _data.BakedData.Texture2);
            Profiler.EndSample();

            Profiler.EndSample();

            // CHECK: Systems seem to be called when exiting playmode once things start getting destroyed, such as the mesh here.
            if (_mesh == null || _material == null) return;

            //indirectArgs[1] = (uint)data.Count;
            _indirectArgs[1] = (uint)count;
            _argsBuffer.SetData(_indirectArgs);

            Graphics.DrawMeshInstancedIndirect(_mesh, 0, _material, new Bounds(Vector3.zero, 1000000 * Vector3.one), _argsBuffer, 0, new MaterialPropertyBlock(), ShadowCastingMode.Off, true);
        }

    }

}