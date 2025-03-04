using System.Collections.Generic;

[System.Serializable]
public class MapData
{
    public int width;                   // ��ͼ���
    public int height;                  // ��ͼ�߶�
    public List<TileData> tiles;        // ��Ƭ�����б�

    public MapData(int width, int height)
    {
        this.width = width;
        this.height = height;
        tiles = new List<TileData>();
    }
}

