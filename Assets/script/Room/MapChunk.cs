using System.Collections.Generic;
using UnityEngine;

public class MapChunk
{
    public Vector2Int Position { get; set; }  // ���������
    public List<Room> Rooms { get; set; }  // �����ڵķ���
    public GameObject ChunkObject { get; set; }  // �������Ϸ�������ڴ�ŷ��䡢���˵ȣ�
}

