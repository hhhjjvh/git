[System.Serializable]
public class TileData
{
    public string tileType; // 瓦片类型 (如 "Ground", "Spike", "Ladder")
    public int x;           // 瓦片的X坐标
    public int y;           // 瓦片的Y坐标

    public TileData(string tileType, int x, int y)
    {
        this.tileType = tileType;
        this.x = x;
        this.y = y;
    }
}

