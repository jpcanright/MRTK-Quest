﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Visualization
{
    public sealed class GPUFieldLines : MonoBehaviour
{

    public EMToGPUInterface GPUInterface;
    
        
    #region Instancing properties

    [SerializeField] int _instanceCount = 1000;

    public int instanceCount {
        get { return _instanceCount; }
    }

    [SerializeField] TubeTemplate _template;

    public TubeTemplate template {
        get { return _template; }
    }

    [SerializeField] float _radius = 0.005f;

    public float radius {
        get { return _radius; }
        set { _radius = value; }
    }

    [SerializeField] float _length = 1;

    public float length {
        get { return _length; }
        set { _length = value; }
    }

    #endregion

    #region Dynamics properties

    [SerializeField] float _spread = 1;

    public float spread {
        get { return _spread; }
        set { _spread = value; }
    }

    [SerializeField] float _noiseFrequency = 0.5f;

    public float noiseFrequency {
        get { return _noiseFrequency; }
        set { _noiseFrequency = value; }
    }

    [SerializeField] Vector3 _noiseMotion = Vector3.up * 0.1f;

    public Vector3 noiseMotion {
        get { return _noiseMotion; }
        set { _noiseMotion = value; }
    }

    #endregion

    #region Material properties

    [SerializeField] Material _material;

    public Material material {
        get { return _material; }
    }

    [SerializeField] CosineGradient _gradient;

    public CosineGradient gradient {
        get { return _gradient; }
        set { _gradient = value; }
    }

    #endregion

    #region Misc properties

    [SerializeField] int _randomSeed;

    public int randomSeed {
        set { _randomSeed = value; }
    }

    #endregion

    #region Hidden attributes

    [SerializeField] ComputeShader _compute;

    #endregion

    #region Private fields

    ComputeBuffer _drawArgsBuffer;
    ComputeBuffer _positionBuffer;
    ComputeBuffer _tangentBuffer;
    ComputeBuffer _normalBuffer;
    bool _materialCloned;
    MaterialPropertyBlock _props;
    Vector3 _noiseOffset;

    #endregion

    #region Compute configurations

    const int kThreadCount = 64;
    int ThreadGroupCount { get { return _instanceCount / kThreadCount; } }
    int InstanceCount { get { return kThreadCount * ThreadGroupCount; } }
    int HistoryLength { get { return _template.segments + 1; } }

    #endregion

    #region MonoBehaviour functions

    void OnValidate()
    {
        _instanceCount = Mathf.Max(kThreadCount, _instanceCount);
        _radius = Mathf.Max(0, _radius);
        _length = Mathf.Max(0, _length);
        _spread = Mathf.Max(0, _spread);
        _noiseFrequency = Mathf.Max(0, _noiseFrequency);
    }

    void Start()
    {
        // Initialize the indirect draw args buffer.
        _drawArgsBuffer = new ComputeBuffer(
            1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments
        );

        _drawArgsBuffer.SetData(new uint[5] {
            _template.mesh.GetIndexCount(0), (uint)InstanceCount, 0, 0, 0
        });

        // Allocate compute buffers.
        _positionBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);
        _tangentBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);
        _normalBuffer = new ComputeBuffer(HistoryLength * InstanceCount, 16);

        // This property block is used only for avoiding an instancing bug.
        _props = new MaterialPropertyBlock();
        _props.SetFloat("_UniqueID", UnityEngine.Random.value);

        // Clone the given material before using.
        _material = new Material(_material);
        _material.name += " (cloned)";
        _materialCloned = true;

        //_noiseOffset = Vector3.one * _randomSeed;
    }

    void OnDestroy()
    {
        if (_drawArgsBuffer != null) _drawArgsBuffer.Release();
        if (_positionBuffer != null) _positionBuffer.Release();
        if (_tangentBuffer != null) _tangentBuffer.Release();
        if (_normalBuffer != null) _normalBuffer.Release();
        if (_materialCloned) Destroy(_material);
    }

    void Update()
    {
        Vector4[] charges = GPUInterface.Charges;
        Vector4[] seeds = FieldLineSeedingList(charges);
        _compute.SetVectorArray("_Charges", charges);
        _compute.SetInt("_ArrayLength", charges.Length);
        _compute.SetVectorArray("_FieldLineSeeds", seeds);
        _compute.SetInt("_SeedArrayLength", seeds.Length);
        
	
        // Warn if there are more charged objects in the scene than the maximum array length
        if (charges.Length > GPUInterface.maxChargeArrayLength)
        {
            Debug.LogWarning(
                $"There are {charges.Length.ToString()} charged objects in the scene, more than the maximum of {GPUInterface.maxChargeArrayLength.ToString()} for which electric flux can be calculated.");
        }
        
        // Invoke the update compute kernel.
        var kernel = _compute.FindKernel("FieldUpdate");

        _compute.SetInt("InstanceCount", InstanceCount);
        _compute.SetInt("HistoryLength", HistoryLength);
        _compute.SetFloat("RandomSeed", _randomSeed);
        _compute.SetFloat("Spread", _spread);
        _compute.SetFloat("StepWidth", _length / _template.segments);
        //_compute.SetFloat("NoiseFrequency", _noiseFrequency);
        //_compute.SetVector("NoiseOffset", _noiseOffset);
        _compute.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);

        _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);

        _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

        // Invoke the reconstruction kernel.
        kernel = _compute.FindKernel("FieldReconstruct");

        _compute.SetBuffer(kernel, "PositionBufferRO", _positionBuffer);
        _compute.SetBuffer(kernel, "TangentBuffer", _tangentBuffer);
        _compute.SetBuffer(kernel, "NormalBuffer", _normalBuffer);

        _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

        // Draw the mesh with instancing.
        _material.SetFloat("_Radius", _radius);

        _material.SetVector("_GradientA", _gradient.coeffsA);
        _material.SetVector("_GradientB", _gradient.coeffsB);
        _material.SetVector("_GradientC", _gradient.coeffsC2);
        _material.SetVector("_GradientD", _gradient.coeffsD2);

        _material.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        _material.SetMatrix("_WorldToLocal", transform.worldToLocalMatrix);

        _material.SetBuffer("_PositionBuffer", _positionBuffer);
        _material.SetBuffer("_TangentBuffer", _tangentBuffer);
        _material.SetBuffer("_NormalBuffer", _normalBuffer);

        _material.SetInt("_InstanceCount", InstanceCount);
        _material.SetInt("_HistoryLength", HistoryLength);
        _material.SetInt("_IndexLimit", HistoryLength);

        Graphics.DrawMeshInstancedIndirect(
            _template.mesh, 0, _material,
            new Bounds(transform.position, transform.lossyScale * 5),
            _drawArgsBuffer, 0, _props, ShadowCastingMode.Off, false
        );

        // Move the noise field.
        //_noiseOffset += _noiseMotion * Time.deltaTime;
    }

    #endregion

    Vector4[] FieldLineSeedingList(Vector4[] charges)
    {
        float chargeScaleUnit = 1;
        List<Vector4> output = new List<Vector4>();
        int index = 0;
        foreach (Vector4 charge in charges)
        {
            int entries = Math.Max(1, Mathf.Abs(Mathf.RoundToInt(charge.w / chargeScaleUnit)));
            for (int i = 0; i < entries; i++)
            {
                output.Add(new Vector4(charge.x, charge.y, charge.z, Math.Sign(charge.w)));
                index++;
                if (index == 128)
                {
                    return output.ToArray();
                }
            }
        }
        return output.ToArray();
    }
    
    public void ResetPositions()
    {
        if (_positionBuffer != null)
        {
            // Invoke the initialization kernel.
            var kernel = _compute.FindKernel("FieldLineInit");
            _compute.SetInt("InstanceCount", InstanceCount);
            _compute.SetInt("HistoryLength", HistoryLength);
            _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
            _compute.SetBuffer(kernel, "TangentBuffer", _tangentBuffer);
            _compute.SetBuffer(kernel, "NormalBuffer", _normalBuffer);
            _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);
            _compute.SetBuffer(kernel, "PositionBufferRO", _positionBuffer);
        }
    }
        
}
}


