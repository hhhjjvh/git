using System.Collections.Generic;

[System.Serializable]
public class MapData
{
    public int width;                   // 地图宽度
    public int height;                  // 地图高度
    public List<TileData> tiles;        // 瓦片数据列表

    public MapData(int width, int height)
    {
        this.width = width;
        this.height = height;
        tiles = new List<TileData>();
    }
}

