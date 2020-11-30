using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    [Range(1, 5)]
    public int visibilityRange = 1;
    
    [Range(2, 128)]
    public int chunkLength = 32;
    public ChunkParameters chunkParameters;

    public GameObject playerObject;

    private float halfSize;
    
    private Dictionary<Vector2Int, Chunk> visibleChunks = new Dictionary<Vector2Int, Chunk>();

    private void Update()
    {
        Vector3 playerPosition = playerObject.transform.position;
        Vector2Int chunkPosition = WorldPositionToChunkPosition(playerPosition);

        for (int i = -visibilityRange; i <= visibilityRange; i++)
        {
            for (int j = -visibilityRange; j <= visibilityRange; j++)
            {
                Vector2Int newChunkPosition = chunkPosition + new Vector2Int(i, j);
                if (!visibleChunks.ContainsKey(newChunkPosition))
                {
                    Vector3 chunkWorldPosition = ChunkPositionToWorldPosition(newChunkPosition);
                    Chunk chunk = InstantiateChunk(chunkWorldPosition);
                    visibleChunks.Add(newChunkPosition, chunk);
                }
            }
        }
    }

    Vector2Int WorldPositionToChunkPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / chunkParameters.chunkSize);
        int z = Mathf.FloorToInt(worldPosition.z / chunkParameters.chunkSize);
        return new Vector2Int(x, z);
    }

    Vector3 ChunkPositionToWorldPosition(Vector2Int chunkPosition)
    {
        return new Vector3(chunkPosition.x * chunkParameters.chunkSize, 0f, chunkPosition.y * chunkParameters.chunkSize);
    }

    private Chunk InstantiateChunk(Vector3 position)
    {
        GameObject chunkGameObject = new GameObject();
        chunkGameObject.transform.parent = gameObject.transform;
        chunkGameObject.transform.position = position;
        chunkGameObject.AddComponent<MeshFilter>();
        chunkGameObject.AddComponent<MeshRenderer>();
        chunkGameObject.AddComponent<MeshCollider>();
        Chunk chunk = chunkGameObject.AddComponent<Chunk>();
        chunk.SetParameters(chunkParameters, chunkLength);
        return chunk;
    }
}
