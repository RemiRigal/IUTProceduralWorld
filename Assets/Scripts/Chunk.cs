using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Range(2, 256)]
    public int chunkLength = 64;
    public ChunkParameters parameters;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetParameters(ChunkParameters parameters, int chunkLength)
    {
        this.parameters = parameters;
        this.chunkLength = chunkLength;
        
        ChunkGenerator generator = new ChunkGenerator(parameters);
        // Noise map generation
        float[,] noiseMap = generator.GenerateNoiseMap(chunkLength, transform.position);
        // Texture generation
        Texture2D texture = generator.GenerateTexture(chunkLength, noiseMap);
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        meshRenderer.sharedMaterial.mainTexture = texture;
        // Mesh generation
        Mesh mesh = generator.GenerateMesh(chunkLength, noiseMap);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}