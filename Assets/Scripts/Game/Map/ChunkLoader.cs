using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private int chunkSize = 10;
    [SerializeField] private float loadDistance = 50f;

    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector2Int currentChunk = GetChunkPositionFromWorldPoint(mainCamera.transform.position);

        LoadChunksAroundPosition(currentChunk);
        UnloadChunksOutsideRange(currentChunk);
    }

    private void LoadChunksAroundPosition(Vector2Int position)
    {
        int minX = Mathf.FloorToInt(position.x / chunkSize);
        int maxX = Mathf.CeilToInt(position.x / chunkSize);
        int minY = Mathf.FloorToInt(position.y / chunkSize);
        int maxY = Mathf.CeilToInt(position.y / chunkSize);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                if (!IsChunkLoaded(chunkPosition))
                {
                    LoadChunk(chunkPosition);
                }
            }
        }
    }

    private void UnloadChunksOutsideRange(Vector2Int currentChunk)
    {
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, GameObject> chunk in loadedChunks)
        {
            Vector2Int chunkPosition = chunk.Key;
            if (Vector2Int.Distance(chunkPosition, currentChunk) > loadDistance)
            {
                UnloadChunk(chunkPosition);
                chunksToRemove.Add(chunkPosition);
            }
        }

        foreach (Vector2Int chunkPosition in chunksToRemove)
        {
            loadedChunks.Remove(chunkPosition);
        }
    }

    private bool IsChunkLoaded(Vector2Int position)
    {
        return loadedChunks.ContainsKey(position);
    }

    private void LoadChunk(Vector2Int position)
    {
        GameObject chunk = Instantiate(chunkPrefab, GetWorldPointFromChunkPosition(position), Quaternion.identity);
        chunk.name = "Chunk_" + position.x + "_" + position.y;
        loadedChunks.Add(position, chunk);
    }

    private void UnloadChunk(Vector2Int position)
    {
        if (loadedChunks.TryGetValue(position, out GameObject chunk))
        {
            Destroy(chunk);
        }
    }

    private Vector2Int GetChunkPositionFromWorldPoint(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    private Vector3 GetWorldPointFromChunkPosition(Vector2Int position)
    {
        float x = position.x * chunkSize;
        float y = position.y * chunkSize;
        return new Vector3(x, y, 0f);
    }
}

