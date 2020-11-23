using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
// This class holds all the parameters for the chunks
public class ChunkParameters
{
    [Range(1f, 100f)]
    public float chunkSize = 10f; // Size of the chunk

    public bool lowPoly = false; // Wether or not the mesh should be 'low-poly'
    
    [Range(0.001f, 1f)]
    public float[] noiseScale = {0.1f}; // Aarray of the scale (level of details) for all noise layers
    [Range(0f, 100f)]
    public float[] noiseFactors = {1f}; // Array of the factors (importance) for all noise layers

    [Range(0f, 100f)]
    public float elevationFactor = 10f; // Global elevation multiplier
    public AnimationCurve elevationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // Elevation curve
}
