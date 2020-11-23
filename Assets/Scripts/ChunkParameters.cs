using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChunkParameters
{
    [Range(1f, 100f)]
    public float chunkSize = 10f;

    public bool lowPoly = false;
    
    [Range(0.001f, 1f)]
    public float[] noiseScale = {0.1f};
    [Range(0f, 100f)]
    public float[] noiseFactors = {1f};

    [Range(0f, 100f)]
    public float elevationFactor = 10f;
    public AnimationCurve elevationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
