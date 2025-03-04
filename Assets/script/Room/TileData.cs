[System.Serializable]
public class TileData
{
    public string tileType; // ��Ƭ���� (�� "Ground", "Spike", "Ladder")
    public int x;           // ��Ƭ��X����
    public int y;           // ��Ƭ��Y����

    public TileData(string tileType, int x, int y)
    {
        this.tileType = tileType;
        this.x = x;
        this.y = y;
    }
}

