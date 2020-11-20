using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public ChunkParameters parameters;
    
    // The MeshFilter instance of the GameObject holding the mesh
    private MeshFilter meshFilter;
    
    // The MeshRenderer instance of the GameObject holding the material and texture of the mesh
    private MeshRenderer meshRenderer;

    private ChunkGenerator generator;

    private int currentChunkLength = 0;
    
    private readonly Dictionary<int, float[,]> noiseMaps = new Dictionary<int, float[,]>();
    private readonly Dictionary<int, Mesh> meshes = new Dictionary<int, Mesh>();
    private readonly Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();
    
    
    public void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetChunkLength(int chunkLength)
    {
        if (currentChunkLength == chunkLength)
        {
            return;
        }
        if (generator == null)
        {
            generator = new ChunkGenerator(parameters);
        }

        currentChunkLength = chunkLength;
        // Noise map
        if (!noiseMaps.ContainsKey(chunkLength))
        {
            float[,] noiseMap = generator.GenerateNoiseMap(chunkLength, transform.position);
            noiseMaps.Add(chunkLength, noiseMap);
        }
        // Mesh
        if (!meshes.ContainsKey(chunkLength))
        {
            Mesh mesh = generator.GenerateChunk(chunkLength, noiseMaps[chunkLength]);
            meshes.Add(chunkLength, mesh);
        }
        meshFilter.sharedMesh = meshes[chunkLength];
        // Texture
        if (!textures.ContainsKey(chunkLength))
        {
            Texture2D texture = generator.GenerateTexture(chunkLength, noiseMaps[chunkLength]);
            textures.Add(chunkLength, texture);
        }
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard")) {mainTexture = textures[chunkLength]};
    }
}
