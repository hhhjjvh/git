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

    // 保存地图
    public void SaveMap()
    {
        // 初始化 MapData
        MapData mapData = new MapData(groundTilemap.size.x, groundTilemap.size.y);

        // 保存地面瓦片
        SaveTilemapData(groundTilemap, "Ground", mapData);

        // 保存梯子瓦片
        SaveTilemapData(ladderTilemap, "Ladder", mapData);

        // 保存障碍瓦片
        SaveTilemapData(obstacleTilemap, "Spike", mapData);

        // 序列化为 JSON
        string json = JsonUtility.ToJson(mapData, true);

        // 保存到文件
        File.WriteAllText(savePath, json);
        Debug.Log($"地图保存成功！路径: {savePath}");
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

    // 从 JSON 文件加载地图
    public void LoadMap()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogError($"文件不存在：{savePath}");
            return;
        }

        // 从文件读取 JSON
        string json = File.ReadAllText(savePath);

        // 反序列化为 MapData
        MapData mapData = JsonUtility.FromJson<MapData>(json);

        // 清空现有地图
        groundTilemap.ClearAllTiles();
        ladderTilemap.ClearAllTiles();
        obstacleTilemap.ClearAllTiles();

        // 根据数据重建地图
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

        Debug.Log("地图加载成功！");
    }
}
