using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkGenerator
{
    private ChunkParameters parameters;

    public ChunkGenerator(ChunkParameters param)
    {
        this.parameters = param;
    }

    public float[,] GenerateNoiseMap(int chunkLength, Vector3 initialPosition)
    {
        float[,] map = new float[chunkLength, chunkLength];
        float delta = parameters.chunkSize / (chunkLength - 1);
        for (int i = 0; i < chunkLength; i++)
        {
            for (int j = 0; j < chunkLength; j++)
            {
                float noise = 0f;
                float perlinX = j * delta + initialPosition.x;
                float perlinY = i * delta + initialPosition.z;
                for (int layer = 0; layer < parameters.noiseScale.Length; layer++)
                {
                    float noiseScale = parameters.noiseScale[layer];
                    float noiseFactor = parameters.noiseFactors[layer];
                    noise += noiseFactor * Mathf.PerlinNoise(perlinX * noiseScale, perlinY * noiseScale);
                }
                map[i, j] = noise;
            }
        }
        return map;
    }
    
    public Texture2D GenerateTexture(int chunkLength, float[,] noiseMap)
    {
        Texture2D texture = new Texture2D(chunkLength, chunkLength);
        texture.filterMode = FilterMode.Point;
        float globalFactor = parameters.noiseFactors.Sum();

        for (int i = 0; i < chunkLength; i++)
        {
            for (int j = 0; j < chunkLength; j++)
            {
                float noise = noiseMap[i, j];
                Color color = Color.Lerp(Color.black, Color.white, noise / globalFactor);
                texture.SetPixel(i, j, color);
            }
        }
        
        texture.Apply();
        return texture;
    }
    
    public Mesh GenerateMesh(int chunkLength, float[,] noiseMap)
    {
        Mesh mesh = new Mesh();

        float delta = parameters.chunkSize / (chunkLength - 1);
        float globalFactor = parameters.noiseFactors.Sum();
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        
        for (int i = 0; i < chunkLength; i++)
        {
            for (int j = 0; j < chunkLength; j++)
            {
                float altitude = parameters.elevationCurve.Evaluate(noiseMap[i, j] / globalFactor) * parameters.elevationFactor;
                Vector3 vertex = new Vector3(j * delta, altitude, i * delta);
                vertices.Add(vertex);
                
                Vector2 uvCoordinate = new Vector2((float)i / chunkLength, (float)j / chunkLength);
                uvs.Add(uvCoordinate);

                if (i < chunkLength - 1 && j < chunkLength - 1)
                {
                    triangles.Add(i * chunkLength + j);
                    triangles.Add((i + 1) * chunkLength + j + 1);
                    triangles.Add(i * chunkLength + j + 1);
                    
                    triangles.Add((i + 1) * chunkLength + j + 1);
                    triangles.Add(i * chunkLength + j);
                    triangles.Add((i + 1) * chunkLength + j);
                }
            }
        }
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        return mesh;
    }
}
