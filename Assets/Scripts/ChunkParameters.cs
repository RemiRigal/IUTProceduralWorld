using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class ChunkParameters
{
    [Header("Geometry")]
    [Range(0f, 50f)]
    public float elevationFactor = 1f;
    public AnimationCurve elevationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    
    [Header("Mesh")]
    [Range(1f, 10000f)]
    public float chunkSize = 10f;
    public bool lowPoly = false;
    
    [Header("Noise Map")]
    public float[] noiseScales = {0.1f};
    public float[] noiseFactors = {0.1f};

    public float globalNoiseFactor
    {
        get
        {
            return noiseFactors.Sum();
        }
    }
    
    [Header("Texture")]
    public bool showTexture = true;
}
