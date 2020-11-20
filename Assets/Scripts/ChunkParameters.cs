using System;
using UnityEngine;

[Serializable]
public class ChunkParameters
{
    public bool lowPoly = false;
    
    [Range(0f, 50f)]
    public float elevationFactor = 1f;
    public AnimationCurve elevationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    
    [Range(1f, 100f)]
    public float chunkSize = 10f;
    
    [Header("Noise Map")]
    public float[] noiseScales = {0.1f};
    public float[] noiseFactors = {0.1f};
}
