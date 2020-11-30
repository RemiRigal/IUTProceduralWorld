using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// This class contains all scripts needed to generate a chunk of terrain
public class ChunkGenerator
{
    private ChunkParameters parameters;
    
    private readonly List<Vector2Int> tri = new List<Vector2Int>
    {
        new Vector2Int(0, 0),
        new Vector2Int(1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(0, 0),
        new Vector2Int(1, 0)
    };

    // Constructor of the object, each instance is linked with a ChunkParameters instance
    public ChunkGenerator(ChunkParameters param)
    {
        this.parameters = param;
    }

    // Generates a noise map with a size of 'chunkLength'
    // The noise map is generated according to a reference position (position in the world space of the (0, 0) local coordinate)
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
                // For loops used for the noise layers
                for (int layer = 0; layer < parameters.noiseScale.Length; layer++)
                {
                    float noiseScale = parameters.noiseScale[layer];
                    float noiseFactor = parameters.noiseFactors[layer];
                    noise += noiseFactor * Mathf.PerlinNoise(10000f + perlinX * noiseScale, 10000f + perlinY * noiseScale);
                }
                map[i, j] = noise;
            }
        }
        return map;
    }
    
    // Generates a black and white with a size of 'chunkLength' according to a 'noiseMap'
    public Texture2D GenerateTexture(int chunkLength, float[,] noiseMap)
    {
        Texture2D texture = new Texture2D(chunkLength, chunkLength);
        texture.filterMode = FilterMode.Point;
        // The 'globalFactor' is needed to ensure that the noise value is between 0 and 1
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
    
    // Generates a mesh with a size of 'chunkLength' according a 'noiseMap' as a height map
    public Mesh GenerateMesh(int chunkLength, float[,] noiseMap)
    {
        Mesh mesh = new Mesh();

        float delta = parameters.chunkSize / (chunkLength - 1);
        // The 'globalFactor' is needed to ensure that the noise value is between 0 and 1
        float globalFactor = parameters.noiseFactors.Sum();
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        
        for (int i = 0; i < chunkLength; i++)
        {
            for (int j = 0; j < chunkLength; j++)
            {

                if (parameters.lowPoly)
                {
                    // LowPoly
                    if (i < chunkLength - 1 && j < chunkLength - 1)
                    {
                        foreach (Vector2Int triDelta in tri)
                        {
                            int newI = i + triDelta.x;
                            int newJ = j + triDelta.y;
                        
                            triangles.Add(vertices.Count);
                            float altitude = parameters.elevationCurve.Evaluate(noiseMap[newI, newJ] / globalFactor) * parameters.elevationFactor;
                            Vector3 vertex = new Vector3(newJ * delta, altitude, newI * delta);
                            vertices.Add(vertex);
                            Vector2 uvCoordinate = new Vector2((float)newI / chunkLength, (float)newJ / chunkLength);
                            uvs.Add(uvCoordinate);
                        }
                    }
                }
                else
                {
                    // Standard
                    // The normalized noise value is used to evaluate the elevation curve
                    // The multiplier 'elevationFactor' is then applied to the final altitude
                    float altitude = parameters.elevationCurve.Evaluate(noiseMap[i, j] / globalFactor) * parameters.elevationFactor;
                    Vector3 vertex = new Vector3(j * delta, altitude, i * delta);
                    vertices.Add(vertex);
                
                    Vector2 uvCoordinate = new Vector2((float)i / chunkLength, (float)j / chunkLength);
                    uvs.Add(uvCoordinate);

                    if (i < chunkLength - 1 && j < chunkLength - 1)
                    {
                        triangles.AddRange(tri.Select(triDelta => (i + triDelta.x) * chunkLength + (j + triDelta.y)));
                    }
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
