using System.Collections.Generic;
using UnityEngine;

public class MapChunk
{
    public Vector2Int Position { get; set; }  // 区域的坐标
    public List<Room> Rooms { get; set; }  // 区域内的房间
    public GameObject ChunkObject { get; set; }  // 区域的游戏对象（用于存放房间、敌人等）
}

