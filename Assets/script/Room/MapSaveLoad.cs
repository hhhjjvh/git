using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSaveLoad : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap ladderTilemap;
    public Tilemap obstacleTilemap;
    public Tile groundTile;
    public Tile ladderTile;
    public Tile spikeTile;

    public string savePath = "Assets/Resources/mapData.json";

    // �����ͼ
    public void SaveMap()
    {
        // ��ʼ�� MapData
        MapData mapData = new MapData(groundTilemap.size.x, groundTilemap.size.y);

        // ���������Ƭ
        SaveTilemapData(groundTilemap, "Ground", mapData);

        // ����������Ƭ
        SaveTilemapData(ladderTilemap, "Ladder", mapData);

        // �����ϰ���Ƭ
        SaveTilemapData(obstacleTilemap, "Spike", mapData);

        // ���л�Ϊ JSON
        string json = JsonUtility.ToJson(mapData, true);

        // ���浽�ļ�
        File.WriteAllText(savePath, json);
        Debug.Log($"��ͼ����ɹ���·��: {savePath}");
    }

    private void SaveTilemapData(Tilemap tilemap, string tileType, MapData mapData)
    {
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position))
            {
                mapData.tiles.Add(new TileData(tileType, position.x, position.y));
            }
        }
    }

    // �� JSON �ļ����ص�ͼ
    public void LoadMap()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogError($"�ļ������ڣ�{savePath}");
            return;
        }

        // ���ļ���ȡ JSON
        string json = File.ReadAllText(savePath);

        // �����л�Ϊ MapData
        MapData mapData = JsonUtility.FromJson<MapData>(json);

        // ������е�ͼ
        groundTilemap.ClearAllTiles();
        ladderTilemap.ClearAllTiles();
        obstacleTilemap.ClearAllTiles();

        // ���������ؽ���ͼ
        foreach (TileData tileData in mapData.tiles)
        {
            Vector3Int position = new Vector3Int(tileData.x, tileData.y, 0);
            switch (tileData.tileType)
            {
                case "Ground":
                    groundTilemap.SetTile(position, groundTile);
                    break;
                case "Ladder":
                    ladderTilemap.SetTile(position, ladderTile);
                    break;
                case "Spike":
                    obstacleTilemap.SetTile(position, spikeTile);
                    break;
            }
        }

        Debug.Log("��ͼ���سɹ���");
    }
}
