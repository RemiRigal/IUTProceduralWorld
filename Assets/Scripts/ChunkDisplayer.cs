using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDisplayer : MonoBehaviour
{
    [Range(2, 512)]
    public int chunkLength = 256;
    
    public ChunkParameters parameters;
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    
    private void OnValidate()
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
        float[,] noiseMap = generator.GenerateNoiseMap(chunkLength, Vector3.zero);
        // Mesh
        Mesh mesh = generator.GenerateChunk(chunkLength, noiseMap);
        meshFilter.sharedMesh = mesh;
        // Texture
        Texture2D texture = generator.GenerateTexture(chunkLength, noiseMap);
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard")) {mainTexture = texture};
    }
}
