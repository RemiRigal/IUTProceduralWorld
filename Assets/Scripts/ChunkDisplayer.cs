using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Component can be used to display a chunk in the editor without starting the game
public class ChunkDisplayer : MonoBehaviour
{
    [Range(2, 256)]
    public int chunkLength = 64;
    public ChunkParameters parameters;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    
    void OnValidate()
    {
        // Called whenever a parameter has been updated
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
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
    }
}
