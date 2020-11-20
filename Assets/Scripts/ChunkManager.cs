using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    [Header("Manager properties")]
    [Range(0, 10)]
    public int visibilityRange = 3;
    public GameObject player;
    
    [Header("Resolution")]
    [Range(2, 512)]
    public int chunkLength = 256;
    public bool decimate = true;
    [Range(0.1f, 2f)]
    public float decimationFactor = 1f;

    public ChunkParameters chunkParameters;

    private List<Chunk> visibleChunks = new List<Chunk>();
    private readonly Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    void Update()
    {
        Vector2Int referencePosition = worldToChunkPosition(player.transform.position);
        List<Chunk> newVisibleChunks = new List<Chunk>();

        for (int i = -visibilityRange; i < visibilityRange + 1; i++)
        {
            for (int j = -visibilityRange; j < visibilityRange + 1; j++)
            {
                Vector2Int chunkPosition = referencePosition + new Vector2Int(i, j);
                Chunk chunk;
                if (!chunks.ContainsKey(chunkPosition))
                {
                    Vector3 worldPosition = chunkToWorldPosition(chunkPosition);
                    chunk = SpawnChunk(worldPosition, $"Chunk({i},{j})");
                    chunks.Add(chunkPosition, chunk);
                }
                else
                {
                    chunk = chunks[chunkPosition];
                }
                int length = chunkLength;
                if (decimate)
                {
                    float distance = new Vector2(i, j).magnitude * decimationFactor;
                    length = Mathf.Max(2, chunkLength / Mathf.NextPowerOfTwo(1 + Mathf.RoundToInt(distance)));
                }
                chunk.gameObject.SetActive(true);
                chunk.SetChunkLength(length);
                newVisibleChunks.Add(chunk);
            }
        }

        foreach (Chunk chunk in visibleChunks.Except(newVisibleChunks))
        {
            chunk.gameObject.SetActive(false);
        }
        visibleChunks = newVisibleChunks;
    }

    Chunk SpawnChunk(Vector3 position, string chunkName)
    {
        GameObject chunkObject = new GameObject {name = chunkName};
        chunkObject.transform.position = position;
        chunkObject.transform.parent = transform;
        chunkObject.AddComponent<MeshFilter>();
        chunkObject.AddComponent<MeshRenderer>();
        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.parameters = chunkParameters;
        return chunk;
    }

    Vector2Int worldToChunkPosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x / chunkParameters.chunkSize), Mathf.RoundToInt(worldPosition.z / chunkParameters.chunkSize));
    }

    Vector3 chunkToWorldPosition(Vector2Int chunkPosition)
    {
        return new Vector3((chunkPosition.x - 0.5f) * chunkParameters.chunkSize, 0f, (chunkPosition.y - 0.5f) * chunkParameters.chunkSize);
    }

    private void OnValidate()
    {
        List<Chunk> chunksToDestroy = chunks.Values.ToList();
        chunks.Clear();
        foreach (Chunk chunk in chunksToDestroy)
        {
            Destroy(chunk.gameObject);
            visibleChunks.Remove(chunk);
        }
    }
}
