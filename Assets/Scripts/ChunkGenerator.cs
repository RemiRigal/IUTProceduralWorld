using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkGenerator
{
    
    private ChunkParameters parameters;
    
    private readonly List<Vector2Int> intermediateTriangles = new List<Vector2Int>
    {
        new Vector2Int(0, 0),
        new Vector2Int(1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(0, 0),
        new Vector2Int(1, 0),
    };

    public ChunkGenerator(ChunkParameters parameters)
    {
        this.parameters = parameters;
    }
    
    public float[,] GenerateNoiseMap(int chunkLength, Vector3 initialPosition)
    {
        float[,] map = new float[chunkLength, chunkLength];
        float delta = parameters.chunkSize / (chunkLength - 1);

        float globalFactor = parameters.noiseFactors.Sum();
        for (int i = 0; i < chunkLength; i++)
        {
            for (int j = 0; j < chunkLength; j++)
            {
                float noise = 0f;
                for (int layer = 0; layer < parameters.noiseFactors.Length; layer++)
                {
                    float noiseScale = parameters.noiseScales[layer];
                    float noiseFactor = parameters.noiseFactors[layer];
                    Vector2 noiseVertex = new Vector2(j * delta + initialPosition.x, i * delta + initialPosition.z);
                    noise += noiseFactor * Mathf.PerlinNoise(noiseScale * noiseVertex.x + 1000f, noiseScale * noiseVertex.y + 1000f);
                }
                map[i, j] = noise / globalFactor;
            }
        }
        return map;
    }
    
    public Texture2D GenerateTexture(int chunkLength, float[,] noiseMap)
    {
        Texture2D texture = new Texture2D(chunkLength, chunkLength);
        texture.filterMode = FilterMode.Point;
        
        for (int i = 0; i < chunkLength; i++)
        {
            for (int j = 0; j < chunkLength; j++)
            {
                Color color = Color.Lerp(Color.black, Color.white, noiseMap[i, j]);
                texture.SetPixel(i, j, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    public Mesh GenerateChunk(int chunkLength, float[,] noiseMap)
    {
        // New Mesh instance
        Mesh mesh = new Mesh();

        // The distance between two vertices is the total size divided by the number of vertices minus 1
        float delta = parameters.chunkSize / (chunkLength - 1);
        
        // Those lists contains the vertices, the triangles and the uv coordinates of the grid
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // Main loops iterating through all the vertices
        for (int i = 0; i < chunkLength; i++)
        {
            for (int j = 0; j < chunkLength; j++)
            {
                // Low Poly
                if (parameters.lowPoly)
                {
                    if (i < chunkLength - 1 && j < chunkLength - 1)
                    {
                        foreach (Vector2Int triangle in intermediateTriangles)
                        {
                            int newI = i + triangle.x;
                            int newJ = j + triangle.y;
                            float height = parameters.elevationCurve.Evaluate(noiseMap[newI, newJ]) * parameters.elevationFactor;
                            triangles.Add(vertices.Count);
                            vertices.Add(new Vector3(newJ * delta, height, newI * delta));
                            uvs.Add(new Vector2((float)newI / chunkLength, (float)newJ / chunkLength));
                        }
                    }
                }
                else
                // Standard
                {
                    float height = parameters.elevationCurve.Evaluate(noiseMap[i, j]) * parameters.elevationFactor;
                    vertices.Add(new Vector3(j * delta, height, i * delta));
                    uvs.Add(new Vector2((float)i / chunkLength, (float)j / chunkLength));
                    if (i < chunkLength - 1 && j < chunkLength - 1)
                    {
                        foreach (Vector2Int triangle in intermediateTriangles)
                        {
                            triangles.Add((i + triangle.x) * chunkLength + (j + triangle.y));
                        }
                    }
                }
            }
        }
        
        // Those three functions links the three lists we just made with the mesh instance
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);

        // It is necessary to call the 'RecalculateNormals' function for a mesh
        mesh.RecalculateNormals();
        return mesh;
    }
}
