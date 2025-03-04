using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomMapGenerator : MonoBehaviour
{
    public static RandomMapGenerator Instance { get; private set; }

    public int width = 100; // 地图宽度
    public int height = 20; // 地图高度
    public int roomWidthMin = 36; // 房间最小宽度
    public int roomWidthMax = 40; // 房间最大宽度
    public int roomHeightMin = 20; // 房间最小高度
    public int roomHeightMax = 24; // 房间最大高度

    public int minRoomCount = 4; // 最少房间数量
    public int maxRoomCount = 7; // 最多房间数量

    public Tilemap groundTilemap, wallTilemap, spikesTilemap, platformTilemap, ladderTilemap, doorTilemap;
    public Tile groundTile, wallTile, spikesTile, platformTile, ladderTile, doorOpenTile, doorClosedTile;

    private int[,] groundLayer, wallLayer, spikesLayer, platformLayer, ladderLayer, doorLayer;
    public int seed = 0; // 随机种子
    public int level = 1; // 关卡数

    // public GameObject boungs;
    private List<Room> rooms; // 保存所有房间
    public GameObject enemyPrefab; // 敌人预制体
    public Room currentRoom; // 当前玩家所在的房间
    public Room CheckRoom; // 检查房间
                           // private ObjectPool<GameObject> triggerPool;
    float time;
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // 初始化触发器对象池
        //triggerPool = new ObjectPool<GameObject>(
        //    createFunc: () => Instantiate(boungs), // 使用预制体创建对象
        //    actionOnGet: obj => obj.SetActive(true),
        //    actionOnRelease: obj => obj.SetActive(false),
        //    actionOnDestroy: Destroy,
        //    defaultCapacity: 10,
        //    maxSize: 50
        //);
       
        time = 2;
        PlayerManager.instance.player.transform.position = new Vector3(10, 10, 0);
        GenerateMap();
        RenderTilemaps();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            LevelUpRoom();

        }
        if(time>=0)
        {
            time -= Time.deltaTime;
        }
        if (time < 0)
        // 玩家所在位置
        {
            Vector3 playerPosition = PlayerManager.instance.player.transform.position;
            currentRoom = GetCurrentRoom(playerPosition);
            EnterRoom(currentRoom);
            if (CheckRoom != null && CheckRoom != currentRoom)
            {
                Vector2Int center = CheckRoom.Center;
                PlayerManager.instance.player.transform.position = new Vector3(center.x, center.y, 0);
            }
            if (currentRoom != null && !currentRoom.IsCleared)
            {

                // 当前波次的敌人清空后生成下一波
                if (currentRoom.AreAllEnemiesDefeated() && currentRoom.currentWave < currentRoom.totalWaves)
                {
                    currentRoom.SpawnNextWave(
                        new Vector2(currentRoom.Center.x - currentRoom.Width / 2 + 3, currentRoom.Center.y),
                        new Vector2(currentRoom.Center.x + currentRoom.Width / 2 - 3, currentRoom.Center.y + currentRoom.Height / 2)
                    );
                }

                // 如果所有波次完成，开启房间的门
                if (currentRoom.AreAllWavesCompleted())
                {
                    Room room = currentRoom;
                    // currentRoom.OpenAllDoors(doorTilemap, doorOpenTile);
                    CheckRoom = null;
                    StartCoroutine(room.OpenAllDoor(doorTilemap, doorOpenTile));
                    // StartCoroutine(currentRoom.CloseAllDoor(doorTilemap, doorOpenTile));
                    currentRoom.MarkCleared();
                    // Debug.Log($"Room at {currentRoom.Center} cleared, doors opened.");
                    currentRoom.bossUIManager.HideBossUI();
                    if (currentRoom.Type == RoomType.BossRoom)
                    {

                        int randomIndex = Random.Range(currentRoom.Center.x - currentRoom.Width / 2 + 1, currentRoom.Center.x + currentRoom.Width / 2 - 1);
                        int indxy = GetGroundHeight(randomIndex, currentRoom.Center.y - currentRoom.Height / 2, currentRoom.Height) + 4;

                        Vector2 spawnPosition = new Vector2(randomIndex, indxy);
                        GameObject RoomTransfer = PoolMgr.Instance.GetObj("RoomTransfer", spawnPosition);
                        RoomTransfer.transform.SetParent(transform);
                        currentRoom.AddChestBoss();
                        AudioMgr.Instance.PlayMusic("BGM_a_fateful_encounter");
                    }
                    else if (currentRoom.Type == RoomType.NormalRoom)
                    {
                        currentRoom.AddChestNorm();
                    }
                }
                if (currentRoom.Type == RoomType.BossRoom)
                {
                    currentRoom.UpdateBossHealth();
                }

            }
        }
    }

    public void LevelUpRoom()
    {
        level++;
        //AudioMgr.Instance.PlayMusic("BGM_a_fateful_encounter");
        // 清理当前关卡
        ResetLevel();
        GenerateMap();
        RenderTilemaps();
        PlayerManager.instance.player.transform.position = new Vector3(10, 10, 0);
    }

    void ResetLevel()
    {
        CheckRoom =null;
        // 清理地图图层
        groundTilemap.ClearAllTiles();
        spikesTilemap.ClearAllTiles();
        platformTilemap.ClearAllTiles();
        ladderTilemap.ClearAllTiles();
        doorTilemap.ClearAllTiles();

        // 清理动态对象（如敌人、触发器等）
        foreach (Transform child in transform)
        {
            PoolMgr.Instance.Release(child.gameObject);
            //Destroy(child.gameObject);
        }
        //  确保全部对象被销毁
        foreach (Transform child in transform)
        {
            if (child != null)
            {
               // Debug.Log("Child object not destroyed: " + child.name);
                Destroy(child.gameObject);
            }
        }


        //for (int i = 0; i < rooms.Count; i++)
        //{
        //    rooms[i].ReleaseRoomTrigger();
        //}
        // 清理房间列表
        rooms.Clear();
    }
    // 获取当前房间
    Room GetCurrentRoom(Vector3 position)
    {
        foreach (Room room in rooms)
        {
            if (position.x >= room.Center.x - room.Width / 2 + 1 &&
                position.x <= room.Center.x + room.Width / 2 - 1 &&
                position.y >= room.Center.y - room.Height / 2 + 1 &&
                position.y <= room.Center.y + room.Height / 2 - 1)
            {
                return room;
            }
        }
        return null;
    }

    // 进入房间逻辑
    void EnterRoom(Room room)
    {

        if (room != null && !room.IsCleared && !room.isEnemySpawned)
        {
            //Debug.Log($"Player entered room at {room.Center}");
            //PlayerManager.instance.player.transform.position= new Vector3(room.Center.x, room.Center.y, 0);
            if (currentRoom.Type == RoomType.NormalRoom || currentRoom.Type == RoomType.BossRoom)
            {    // 关闭所有门
                //room.CloseAllDoors(doorTilemap, doorClosedTile);
                StartCoroutine(room.CloseAllDoor(doorTilemap, doorClosedTile));
                CheckRoom= room;
                room.InitializeWaveSystem(enemyPrefab, transform);
                if (currentRoom.Type == RoomType.BossRoom)
                {
                    AudioMgr.Instance.PlayMusic("BGM_man of the hour");
                }
            }
            else if (currentRoom.Type == RoomType.TreasureRoom)
            {
                room.InitializeWaveSystem(enemyPrefab, transform);
               // Debug.Log("Special Room: Add logic for shop or treasure.");
            }

            room.isEnemySpawned = true;
        }
    }

    void GenerateMap()
    {
        // 初始化图层数据


        // 随机种子
        seed = Random.Range(1, 100000);
        //  Debug.Log("width: " + width + "height" + height);
        // 生成房间
        rooms = new List<Room>();
        rooms = GenerateRooms(Random.Range(minRoomCount, maxRoomCount));
        //Debug.Log("width: " + width+ "height"+ height);
        groundLayer = new int[width, height];
        wallLayer = new int[width, height];
        spikesLayer = new int[width, height];
        platformLayer = new int[width, height];
        ladderLayer = new int[width, height];
        doorLayer = new int[width, height];
        for (int i = 0; i < rooms.Count; i++)
        {
            GenerateRoom(rooms[i]);
            //Debug.Log(" room type: " + rooms[i].Type);
        }

        // 连接房间并生成门
        for (int i = 0; i < rooms.Count - 1; i++)
        {

            CreateCorridor(rooms[i], rooms[i + 1]);
        }
    }
    List<Room> GenerateRooms(int roomCount)
    {
        List<Room> rooms = new List<Room>();
        Room startRoom = null;
        Room farthestRoom = null;
        float maxDistance = -1;

        int totalWidth = 0;
        int totalHeight = 0;

        for (int i = 0; i < roomCount; i++)
        {
            // 随机位置生成房间
            int roomWidth = Random.Range(roomWidthMin, roomWidthMax);  // 每个房间的随机宽度
            int roomHeight = Random.Range(roomHeightMin, roomHeightMax); // 每个房间的随机高度
            if (roomWidth % 2 == 1) roomWidth++;
            if (roomHeight % 2 == 1) roomHeight++;
            Vector2Int center = new Vector2Int(totalWidth + roomWidth / 2, roomHeight / 2);
            int difficulty = Random.Range(2 * level-1, 4 * level); // 难度范围 [1, 10]
            RoomType type = RoomType.NormalRoom;
            if (i == 0)
            {
                type = RoomType.StartRoom;
                startRoom = new Room(roomWidth, roomHeight, center, type, difficulty);
                rooms.Add(startRoom);
            }
            else
            {
                Room room;
                if (Random.Range(0, 100) < 40)
                {
                    type = RoomType.TreasureRoom;
                    type = Random.Range(0, 2) == 0 ? RoomType.TreasureRoom : RoomType.ShopRoom;
                    room = type == RoomType.ShopRoom
                       ? new ShopRoom(roomWidth, roomHeight, center, difficulty)
                       : new TreasureRoom(roomWidth, roomHeight, center, difficulty);

                }
                else
                {
                    room = new Room(roomWidth, roomHeight, center, type, difficulty);
                }
                rooms.Add(room);

                // 计算房间距离
                float distance = Vector2.Distance(center, startRoom.Center);
                room.DistanceFromStart = distance;

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestRoom = room;
                }
            }

            totalWidth += roomWidth;  // 更新总宽度
            totalHeight = Mathf.Max(totalHeight, roomHeight);  // 更新总高度（取最大值）
        }

        if (farthestRoom != null)
        {
            
            // 增加一个 Boss 房间，而不是替换
            Room bossRoom = new Room(farthestRoom.Width, farthestRoom.Height, 
                farthestRoom.Center,
                RoomType.BossRoom, farthestRoom.Difficulty + 1);
            rooms.Add(bossRoom);
            int newWidth = (int)(farthestRoom.Width * 1.2);
            int newHeight = Random.Range(roomHeightMin, roomHeightMax);
            if (newWidth % 2 == 1) newWidth++;
            if (newHeight % 2 == 1) newHeight++;
            //bossRoom.Center = new Vector2Int(farthestRoom.Center.x + (int)(farthestRoom.Width * .1f),
            //    farthestRoom.Center.y + (int)(farthestRoom.Height * .05f));
            bossRoom.Width = newWidth;  // Boss 房间更大
            bossRoom.Height = newHeight;
            Vector2Int center = new Vector2Int(totalWidth + bossRoom.Width/2, bossRoom.Height/2);
            bossRoom.Center = center;
        }
        // 更新地图的总宽度和高度
        width = totalWidth + 50;  // 加额外空间
        height = totalHeight + 50;

        return rooms;
    }

    public void GenerateJumpPlatforms(Room room, List<ItemData> availableItems)
    {
        int platformCount = availableItems.Count; // 每个物品对应一个平台
        float platformSpacing = (room.Width - 3) / (float)platformCount;  // 根据房间宽度和物品数量设置间隔

        for (int i = 0; i < platformCount; i++)
        {
            // 计算平台的位置
            float platformX = room.Center.x - room.Width / 2 + 5 + i * platformSpacing;
            Vector2 platformPosition = new Vector2(platformX, room.Center.y - room.Height / 2 + 4); // 离地面 4 格

            // 生成单向跳跃平台
            CreatePlatformAtPosition(platformPosition);

            // 为平台上放置一个物品
            ItemData item = availableItems[i];
            SpawnItemOnPlatform(platformPosition, item, room as ShopRoom);
        }
    }

    void CreatePlatformAtPosition(Vector2 position)
    {
        // 在瓦片地图中绘制单向跳跃平台
        int platformWidth = 4;
        int platformHeight = 1;

        for (int x = (int)(position.x - platformWidth / 2); x < (int)(position.x + platformWidth / 2); x++)
        {
            for (int y = (int)position.y; y < (int)position.y + platformHeight; y++)
            {
                platformLayer[x, y] = 1;  // 将瓦片设置为单向跳跃平台
            }
        }
    }

    void SpawnItemOnPlatform(Vector2 platformPosition, ItemData item, ShopRoom shopRoom)
    {
        // 在平台上放置物品
        Vector2 itemPosition = new Vector2(platformPosition.x, platformPosition.y + 0.5f); // 放置在平台中间

        // 假设你有物品生成的函数
        GameObject itemObj = PoolMgr.Instance.GetObj("Item _Sword", itemPosition);
        itemObj.GetComponent<ItemObject>().SetUpItem(item, new Vector2(0, 5f), shopRoom);
        itemObj.transform.SetParent(transform);
    }

    void GenerateRoom(Room room)
    {
        // 使用 Room 的信息直接生成边界
        GenerateBorders(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);

        // 根据房间类型生成内容
        switch (room.Type)
        {
            case RoomType.NormalRoom:
                GenerateGround(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                GeneratePlatforms(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                break;
            case RoomType.BossRoom:
                GenerateGround(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                GeneratePlatforms(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                //Debug.Log("GenerateRoom: " + room.Type+ "roomWidth:"+ room.Width + "roomHeight:" + room.Height);
                break;

            case RoomType.TreasureRoom:
                GenerateGround(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                GenerateSpikes(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                GeneratePlatforms(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                break;
            case RoomType.ShopRoom:
                // GeneratePlatforms(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                // (ShopRoom)room).GenerateItems();
                ((ShopRoom)room).GenerateShopPlatforms();
                break;

            case RoomType.StartRoom:
                // GenerateGround(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);
                break;

            default:
                Debug.LogWarning($"Room type {room.Type} is not handled!");
                break;
        }
        room.CreateRoomTrigger();
    }


    void CreateCorridor(Room roomA, Room roomB, int doorSize = 4, Transform parent = null)
    {
        // if (roomA.Center.y == roomB.Center.y) // 水平方向相连
        // {
        int startX1 = Mathf.Min(roomA.Center.x, roomB.Center.x) + roomA.Width / 2 - 1; // 公共墙壁的起点 X 坐标
        int startX2 = Mathf.Max(roomA.Center.x, roomB.Center.x) - roomB.Width / 2;     // 公共墙壁的终点 X 坐标
        int groundHeightA = GetGroundHeight(startX1 - 1, roomA.Center.y - roomA.Height / 2, roomA.Height);
        int groundHeightB = GetGroundHeight(startX2 + 1, roomB.Center.y - roomB.Height / 2, roomB.Height);
        int clearHeight = Mathf.Max(groundHeightA, groundHeightB) + 1;

        // 删除尖刺和地面瓦片
        spikesLayer[startX1 - 1, groundHeightA + 1] = 0;
        groundLayer[startX1 - 1, groundHeightA + 1] = 0;
        spikesLayer[startX2 + 1, groundHeightB + 1] = 0;
        groundLayer[startX2 + 1, groundHeightB + 1] = 0;

        // 删除墙壁并生成通道
        for (int i = clearHeight; i < clearHeight + doorSize; i++)
        {
            wallLayer[startX1, i] = 0;
            wallLayer[startX2, i] = 0;
            groundLayer[startX1, i] = 0;
            groundLayer[startX2, i] = 0;

            //  Debug.Log($"Deleted wall at: ({startX1}, {i}) and ({startX2}, {i})");
        }

        // 添加门信息到房间
        Door newDoorA = new Door(new Vector2Int(startX1, clearHeight), doorSize, true, transform);
        Door newDoorB = new Door(new Vector2Int(startX2, clearHeight), doorSize, true, transform);

        // ToggleDoor(newDoorA, true);
        // ToggleDoor(newDoorA, true);
        roomA.AddDoor(newDoorA, doorTilemap, doorOpenTile);
        roomB.AddDoor(newDoorB, doorTilemap, doorOpenTile);

        //}
        //else if (roomA.Center.x == roomB.Center.x) // 垂直方向相连
        //{
        //int startY1 = Mathf.Min(roomA.Center.y, roomB.Center.y) + roomHeight / 2 - 1; // 公共墙壁的起点 Y 坐标
        //int startY2 = Mathf.Max(roomA.Center.y, roomB.Center.y) - roomHeight / 2;     // 公共墙壁的终点 Y 坐标

        //// 删除墙壁并生成通道
        //for (int i = roomA.Center.x - 2; i < roomA.Center.x + 2; i++)
        //{
        //    wallLayer[i, startY1] = 0;
        //    wallLayer[i, startY2] = 0;
        //    Debug.Log($"Deleted wall at: ({i}, {startY1}) and ({i}, {startY2})");
        //}
        //int doorX = roomA.Center.x; // 门的 X 坐标
        //int startY = (roomA.Center.y + roomB.Center.y) / 2 - doorSize / 2; // 门的起点 Y 坐标

        //// 生成多瓦片门
        //for (int i = 0; i < doorSize; i++)
        //{
        //    doorTilemap.SetTile(new Vector3Int(doorX, startY + i, 0), doorOpenTile);
        //}

        // 添加门信息到房间
        //Door newDoor = new Door(new Vector2Int(doorX, startY), doorSize, true);
        //roomA.AddDoor(newDoor);
        //roomB.AddDoor(newDoor);
        // }
    }


    void GenerateGround(int startX, int startY, int roomWidth, int roomHeight)
    {
        int lastHeight = startY + (int)(roomHeight * Random.Range(0.1f, 0.8f)); // 初始地面高度

        for (int x = startX; x < startX + roomWidth; x++)
        {
            float noiseValue = Mathf.PerlinNoise((x + seed) * 0.05f, seed * 0.1f);
            int currentHeight = Mathf.FloorToInt(noiseValue * (roomHeight / 4) + lastHeight * 0.5f);

            // 平滑地形，限制高度差变化
            currentHeight = Mathf.Clamp(currentHeight, lastHeight - 1, lastHeight + 1);
            currentHeight = Mathf.Clamp(currentHeight, startY + 2, startY + roomHeight - 2);
            lastHeight = currentHeight;

            // 填充地面瓦片
            for (int y = startY; y <= currentHeight; y++)
            {
                groundLayer[x, y] = 1;
            }
        }
    }

    void GenerateBorders(int startX, int startY, int roomWidth, int roomHeight)
    {
        for (int x = startX; x < startX + roomWidth; x++)
        {
            wallLayer[x, startY] = 1; // 底部边界
            wallLayer[x, startY + roomHeight - 1] = 1; // 顶部边界
        }
        for (int y = startY; y < startY + roomHeight; y++)
        {
            wallLayer[startX, y] = 1; // 左边界
            wallLayer[startX + roomWidth - 1, y] = 1; // 右边界
        }
        //GameObject Bous = Instantiate(boungs, new Vector3(startX + 0.5f, startY + 0.5f, 0), Quaternion.identity);
        //Bous.GetComponent<TriggER>().SetRoomBounds(roomWidth - 1, roomHeight - 1);
    }

    void GenerateSpikes(int startX, int startY, int roomWidth, int roomHeight)
    {
        for (int x = startX + Random.Range(3, 7); x < startX + roomWidth - 1; x++)
        {
            int groundHeight = GetGroundHeight(x, startY, roomHeight);

            // 确保尖刺只生成在地表
            if (groundHeight > 0 && groundHeight < startY + roomHeight - 1)
            {
                int spikeLength = Random.Range(3, 6);

                for (int i = 0; i < spikeLength; i++)
                {
                    int spikeX = x + i;

                    if (spikeX < startX + roomWidth - 1 && GetGroundHeight(spikeX, startY, roomHeight) == groundHeight)
                    {
                        spikesLayer[spikeX, groundHeight + 1] = 1; // 尖刺放置在地表上
                    }
                }

                x += spikeLength + Random.Range(2, 6); // 跳过尖刺段并加随机间隔
            }
        }
    }

    void GeneratePlatforms(int startX, int startY, int roomWidth, int roomHeight)
    {
        int x = startX + Random.Range(3, 7);

        while (x < startX + roomWidth - 8)
        {
            int platformHeight = Random.Range(GetGroundHeight(x, startY, roomHeight) + 4, startY + roomHeight - 4); // 平台高度
            int platformLength = Random.Range(3, 6); // 平台长度

            for (int i = 0; i < platformLength; i++)
            {
                if (x + i < startX + roomWidth - 1)
                {
                    platformLayer[x + i, platformHeight] = 1; // 生成平台
                }
            }

            // 检查是否需要生成楼梯
            int groundHeight = GetGroundHeight(x, startY, roomHeight); // 获取地面高度
            int distanceToGround = platformHeight - groundHeight;

            if (distanceToGround > 8) // 如果平台离地面超过8个瓦片
            {
                if (Random.value < 0.8f) // 80%概率生成楼梯
                {
                    GenerateStairs(x, x + platformLength, platformHeight, startY, roomHeight); // 调用生成楼梯的函数
                }
            }

            x += platformLength + Random.Range(3, 8); // 跳过当前平台段并加随机间隔
        }
    }
    void GenerateStairs(int startX, int endX, int platformHeight, int startY, int roomHeight)
    {
        int groundHeight = GetGroundHeight((startX + endX) / 2, startY, roomHeight); // 获取大致中点的地面高度

        // 检查从 startX 到 endX 的尖刺分布
        List<int> safePositions = new List<int>(); // 记录安全生成梯子的位置
        for (int x = startX; x <= endX; x++)
        {
            int groundHeightAtX = GetGroundHeight(x, startY, roomHeight);
            bool hasSpike = spikesLayer[x, groundHeightAtX + 1] == 1; // 检查地面高度 + 1 是否有尖刺

            if (!hasSpike)
            {
                safePositions.Add(x); // 如果没有尖刺，将此位置标记为安全位置
            }
        }

        // 根据尖刺分布决定生成逻辑
        if (safePositions.Count == 0)
        {
            //Debug.Log("跳过梯子生成：范围内全部是尖刺");
            return; // 全部是尖刺，跳过生成
        }

        // 如果没有尖刺，优先选择中间位置生成
        int ladderX;
        if (safePositions.Count == (endX - startX + 1))
        {
            ladderX = (startX + endX) / 2; // 完全没有尖刺，选择中点
        }
        else
        {
            // 如果有尖刺但有安全区域，选择安全区域中最接近中点的位置
            int middleX = (startX + endX) / 2;
            ladderX = safePositions.OrderBy(x => Mathf.Abs(x - middleX)).First(); // 找到离中点最近的安全位置
        }

        // 确保梯子从地面生成到平台高度
        int groundHeightAtLadder = GetGroundHeight(ladderX, startY, roomHeight);
        for (int y = groundHeightAtLadder + 1; y <= platformHeight; y++)
        {
            ladderLayer[ladderX, y] = 1; // 设置梯子
        }

        // Debug.Log($"梯子生成在位置：{ladderX}，地面高度：{groundHeightAtLadder}，平台高度：{platformHeight}");
    }
    int GetGroundHeight(int x, int startY, int roomHeight)
    {
        for (int y = startY + roomHeight - 1; y >= startY; y--)
        {
            if (groundLayer[x, y] == 1)
            {
                return y;
            }
        }
        return 1;
    }

    void RenderTilemaps()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        spikesTilemap.ClearAllTiles();
        platformTilemap.ClearAllTiles();
        ladderTilemap.ClearAllTiles();
        // doorTilemap.ClearAllTiles();

        RenderTilemap(groundTilemap, groundLayer, groundTile);
        RenderTilemap(wallTilemap, wallLayer, wallTile);
        RenderTilemap(spikesTilemap, spikesLayer, spikesTile);
        RenderTilemap(platformTilemap, platformLayer, platformTile);
        RenderTilemap(ladderTilemap, ladderLayer, ladderTile);
        // RenderTilemap(doorTilemap, doorLayer, doorOpenTile);
    }

    void RenderTilemap(Tilemap tilemap, int[,] layerData, Tile tile)
    {
        for (int x = 0; x < layerData.GetLength(0); x++)
        {
            for (int y = 0; y < layerData.GetLength(1); y++)
            {
                if (layerData[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }
}


