using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapManager : MonoBehaviour
{
    public Transform player;
    public GameObject roomPrefab;
    private List<MapChunk> loadedChunks = new List<MapChunk>();  // �Ѽ��ص�����
    private Vector2Int currentChunkPosition;  // ��ҵ�ǰ��������
    private float chunkSize = 50f;  // ÿ������Ĵ�С�����磺50x50��

    void Update()
    {
        // ÿ֡������λ�ã��ж��Ƿ���Ҫ�����������ж��Զ�������
        Vector2Int playerChunkPosition = GetChunkPosition(player.position);
        if (playerChunkPosition != currentChunkPosition)
        {
            LoadChunksAroundPlayer(playerChunkPosition);
            UnloadDistantChunks(playerChunkPosition);
            currentChunkPosition = playerChunkPosition;
        }
    }

    // ������ҵ�ǰ��������
    private Vector2Int GetChunkPosition(Vector3 playerPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.z / chunkSize)
        );
    }

    // ���������Χ������
    private void LoadChunksAroundPlayer(Vector2Int playerChunkPosition)
    {
        // ���ص�ǰ�����Լ���Χ��8������
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

    // ��������Ƿ��Ѿ�����
    private bool IsChunkLoaded(Vector2Int chunkPosition)
    {
        return loadedChunks.Exists(chunk => chunk.Position == chunkPosition);
    }

    // ���ص�������
    private void LoadChunk(Vector2Int chunkPosition)
    {
        // ����һ���µ��������
        GameObject chunkObject = new GameObject($"Chunk_{chunkPosition.x}_{chunkPosition.y}");
        chunkObject.transform.position = new Vector3(chunkPosition.x * chunkSize, 0, chunkPosition.y * chunkSize);

        // ���ظ�����ķ��䲢�����Ǽ��뵽������
        MapChunk newChunk = new MapChunk { Position = chunkPosition, ChunkObject = chunkObject, Rooms = new List<Room>() };

        // ��������������ɷ���
        for (int i = 0; i < 4; i++)  // ʾ����ÿ����������4������
        {
            Room room = Instantiate(roomPrefab).GetComponent<Room>();
            //room.transform.SetParent(chunkObject.transform);
            //room.transform.localPosition = new Vector3(i * 10f, 0, 0);  // �򵥵����з���
            newChunk.Rooms.Add(room);
        }

        loadedChunks.Add(newChunk);
    }

    // ж�����Զ�������
    private void UnloadDistantChunks(Vector2Int playerChunkPosition)
    {
        List<MapChunk> chunksToUnload = new List<MapChunk>();

        // �ҳ������Χ��������������ж��
        foreach (MapChunk chunk in loadedChunks)
        {
            if (Vector2Int.Distance(chunk.Position, playerChunkPosition) > 1)  // �������1������ľ��룬��ж��
            {
                chunksToUnload.Add(chunk);
            }
        }

        // ж������
        foreach (MapChunk chunk in chunksToUnload)
        {
            Destroy(chunk.ChunkObject);  // ���ٸ��������Ϸ����
            loadedChunks.Remove(chunk);  // ���Ѽ����б����Ƴ�
        }
    }
}
