using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapManager : MonoBehaviour
{
    public Transform player;
    public GameObject roomPrefab;
    private List<MapChunk> loadedChunks = new List<MapChunk>();  // 已加载的区域
    private Vector2Int currentChunkPosition;  // 玩家当前所在区域
    private float chunkSize = 50f;  // 每个区域的大小（例如：50x50）

    void Update()
    {
        // 每帧检查玩家位置，判断是否需要加载新区域或卸载远离的区域
        Vector2Int playerChunkPosition = GetChunkPosition(player.position);
        if (playerChunkPosition != currentChunkPosition)
        {
            LoadChunksAroundPlayer(playerChunkPosition);
            UnloadDistantChunks(playerChunkPosition);
            currentChunkPosition = playerChunkPosition;
        }
    }

    // 计算玩家当前所在区域
    private Vector2Int GetChunkPosition(Vector3 playerPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.z / chunkSize)
        );
    }

    // 加载玩家周围的区域
    private void LoadChunksAroundPlayer(Vector2Int playerChunkPosition)
    {
        // 加载当前区域以及周围的8个区域
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector2Int chunkPosition = playerChunkPosition + new Vector2Int(x, z);
                if (!IsChunkLoaded(chunkPosition))
                {
                    LoadChunk(chunkPosition);
                }
            }
        }
    }

    // 检查区域是否已经加载
    private bool IsChunkLoaded(Vector2Int chunkPosition)
    {
        return loadedChunks.Exists(chunk => chunk.Position == chunkPosition);
    }

    // 加载单个区域
    private void LoadChunk(Vector2Int chunkPosition)
    {
        // 创建一个新的区域对象
        GameObject chunkObject = new GameObject($"Chunk_{chunkPosition.x}_{chunkPosition.y}");
        chunkObject.transform.position = new Vector3(chunkPosition.x * chunkSize, 0, chunkPosition.y * chunkSize);

        // 加载该区域的房间并将它们加入到区域中
        MapChunk newChunk = new MapChunk { Position = chunkPosition, ChunkObject = chunkObject, Rooms = new List<Room>() };

        // 在这个区域中生成房间
        for (int i = 0; i < 4; i++)  // 示例：每个区域生成4个房间
        {
            Room room = Instantiate(roomPrefab).GetComponent<Room>();
            //room.transform.SetParent(chunkObject.transform);
            //room.transform.localPosition = new Vector3(i * 10f, 0, 0);  // 简单地排列房间
            newChunk.Rooms.Add(room);
        }

        loadedChunks.Add(newChunk);
    }

    // 卸载玩家远离的区域
    private void UnloadDistantChunks(Vector2Int playerChunkPosition)
    {
        List<MapChunk> chunksToUnload = new List<MapChunk>();

        // 找出玩家周围区域外的区域进行卸载
        foreach (MapChunk chunk in loadedChunks)
        {
            if (Vector2Int.Distance(chunk.Position, playerChunkPosition) > 1)  // 如果超过1个区域的距离，则卸载
            {
                chunksToUnload.Add(chunk);
            }
        }

        // 卸载区域
        foreach (MapChunk chunk in chunksToUnload)
        {
            Destroy(chunk.ChunkObject);  // 销毁该区域的游戏对象
            loadedChunks.Remove(chunk);  // 从已加载列表中移除
        }
    }
}
