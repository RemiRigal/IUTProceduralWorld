using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDisplayer : MonoBehaviour
{
    [Range(2, 256)]
    public int chunkLength = 64;
    public ChunkParameters parameters;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    
    void DisplayChunk()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        ChunkGenerator generator = new ChunkGenerator(parameters);
        // Noise map
        float[,] noiseMap = generator.GenerateNoiseMap(chunkLength, transform.position);
        // Texture
        Texture2D texture = generator.GenerateTexture(chunkLength, noiseMap);
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        meshRenderer.sharedMaterial.mainTexture = texture;
        // Mesh
        Mesh mesh = generator.GenerateMesh(chunkLength, noiseMap);
        meshFilter.sharedMesh = mesh;
    }
    
    private void OnValidate()
    {
        DisplayChunk();
    }
}
