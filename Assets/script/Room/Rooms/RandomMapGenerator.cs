using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomMapGenerator : MonoBehaviour
{
    public static RandomMapGenerator Instance { get; private set; }

    public int width = 100; // ��ͼ���
    public int height = 20; // ��ͼ�߶�
    public int roomWidthMin = 36; // ������С���
    public int roomWidthMax = 40; // ���������
    public int roomHeightMin = 20; // ������С�߶�
    public int roomHeightMax = 24; // �������߶�

    public int minRoomCount = 4; // ���ٷ�������
    public int maxRoomCount = 7; // ��෿������

    public Tilemap groundTilemap, wallTilemap, spikesTilemap, platformTilemap, ladderTilemap, doorTilemap;
    public Tile groundTile, wallTile, spikesTile, platformTile, ladderTile, doorOpenTile, doorClosedTile;

    private int[,] groundLayer, wallLayer, spikesLayer, platformLayer, ladderLayer, doorLayer;
    public int seed = 0; // �������
    public int level = 1; // �ؿ���

    // public GameObject boungs;
    private List<Room> rooms; // �������з���
    public GameObject enemyPrefab; // ����Ԥ����
    public Room currentRoom; // ��ǰ������ڵķ���
    public Room CheckRoom; // ��鷿��
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
        // ��ʼ�������������
        //triggerPool = new ObjectPool<GameObject>(
        //    createFunc: () => Instantiate(boungs), // ʹ��Ԥ���崴������
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
        // �������λ��
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

                // ��ǰ���εĵ�����պ�������һ��
                if (currentRoom.AreAllEnemiesDefeated() && currentRoom.currentWave < currentRoom.totalWaves)
                {
                    currentRoom.SpawnNextWave(
                        new Vector2(currentRoom.Center.x - currentRoom.Width / 2 + 3, currentRoom.Center.y),
                        new Vector2(currentRoom.Center.x + currentRoom.Width / 2 - 3, currentRoom.Center.y + currentRoom.Height / 2)
                    );
                }

                // ������в�����ɣ������������
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
        // ����ǰ�ؿ�
        ResetLevel();
        GenerateMap();
        RenderTilemaps();
        PlayerManager.instance.player.transform.position = new Vector3(10, 10, 0);
    }

    void ResetLevel()
    {
        CheckRoom =null;
        // �����ͼͼ��
        groundTilemap.ClearAllTiles();
        spikesTilemap.ClearAllTiles();
        platformTilemap.ClearAllTiles();
        ladderTilemap.ClearAllTiles();
        doorTilemap.ClearAllTiles();

        // ����̬��������ˡ��������ȣ�
        foreach (Transform child in transform)
        {
            PoolMgr.Instance.Release(child.gameObject);
            //Destroy(child.gameObject);
        }
        //  ȷ��ȫ����������
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
        // �������б�
        rooms.Clear();
    }
    // ��ȡ��ǰ����
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

    // ���뷿���߼�
    void EnterRoom(Room room)
    {

        if (room != null && !room.IsCleared && !room.isEnemySpawned)
        {
            //Debug.Log($"Player entered room at {room.Center}");
            //PlayerManager.instance.player.transform.position= new Vector3(room.Center.x, room.Center.y, 0);
            if (currentRoom.Type == RoomType.NormalRoom || currentRoom.Type == RoomType.BossRoom)
            {    // �ر�������
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
        // ��ʼ��ͼ������


        // �������
        seed = Random.Range(1, 100000);
        //  Debug.Log("width: " + width + "height" + height);
        // ���ɷ���
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

        // ���ӷ��䲢������
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
            // ���λ�����ɷ���
            int roomWidth = Random.Range(roomWidthMin, roomWidthMax);  // ÿ�������������
            int roomHeight = Random.Range(roomHeightMin, roomHeightMax); // ÿ�����������߶�
            if (roomWidth % 2 == 1) roomWidth++;
            if (roomHeight % 2 == 1) roomHeight++;
            Vector2Int center = new Vector2Int(totalWidth + roomWidth / 2, roomHeight / 2);
            int difficulty = Random.Range(2 * level-1, 4 * level); // �Ѷȷ�Χ [1, 10]
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

                // ���㷿�����
                float distance = Vector2.Distance(center, startRoom.Center);
                room.DistanceFromStart = distance;

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestRoom = room;
                }
            }

            totalWidth += roomWidth;  // �����ܿ��
            totalHeight = Mathf.Max(totalHeight, roomHeight);  // �����ܸ߶ȣ�ȡ���ֵ��
        }

        if (farthestRoom != null)
        {
            
            // ����һ�� Boss ���䣬�������滻
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
            bossRoom.Width = newWidth;  // Boss �������
            bossRoom.Height = newHeight;
            Vector2Int center = new Vector2Int(totalWidth + bossRoom.Width/2, bossRoom.Height/2);
            bossRoom.Center = center;
        }
        // ���µ�ͼ���ܿ�Ⱥ͸߶�
        width = totalWidth + 50;  // �Ӷ���ռ�
        height = totalHeight + 50;

        return rooms;
    }

    public void GenerateJumpPlatforms(Room room, List<ItemData> availableItems)
    {
        int platformCount = availableItems.Count; // ÿ����Ʒ��Ӧһ��ƽ̨
        float platformSpacing = (room.Width - 3) / (float)platformCount;  // ���ݷ����Ⱥ���Ʒ�������ü��

        for (int i = 0; i < platformCount; i++)
        {
            // ����ƽ̨��λ��
            float platformX = room.Center.x - room.Width / 2 + 5 + i * platformSpacing;
            Vector2 platformPosition = new Vector2(platformX, room.Center.y - room.Height / 2 + 4); // ����� 4 ��

            // ���ɵ�����Ծƽ̨
            CreatePlatformAtPosition(platformPosition);

            // Ϊƽ̨�Ϸ���һ����Ʒ
            ItemData item = availableItems[i];
            SpawnItemOnPlatform(platformPosition, item, room as ShopRoom);
        }
    }

    void CreatePlatformAtPosition(Vector2 position)
    {
        // ����Ƭ��ͼ�л��Ƶ�����Ծƽ̨
        int platformWidth = 4;
        int platformHeight = 1;

        for (int x = (int)(position.x - platformWidth / 2); x < (int)(position.x + platformWidth / 2); x++)
        {
            for (int y = (int)position.y; y < (int)position.y + platformHeight; y++)
            {
                platformLayer[x, y] = 1;  // ����Ƭ����Ϊ������Ծƽ̨
            }
        }
    }

    void SpawnItemOnPlatform(Vector2 platformPosition, ItemData item, ShopRoom shopRoom)
    {
        // ��ƽ̨�Ϸ�����Ʒ
        Vector2 itemPosition = new Vector2(platformPosition.x, platformPosition.y + 0.5f); // ������ƽ̨�м�

        // ����������Ʒ���ɵĺ���
        GameObject itemObj = PoolMgr.Instance.GetObj("Item _Sword", itemPosition);
        itemObj.GetComponent<ItemObject>().SetUpItem(item, new Vector2(0, 5f), shopRoom);
        itemObj.transform.SetParent(transform);
    }

    void GenerateRoom(Room room)
    {
        // ʹ�� Room ����Ϣֱ�����ɱ߽�
        GenerateBorders(room.Center.x - room.Width / 2, room.Center.y - room.Height / 2, room.Width, room.Height);

        // ���ݷ���������������
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
        // if (roomA.Center.y == roomB.Center.y) // ˮƽ��������
        // {
        int startX1 = Mathf.Min(roomA.Center.x, roomB.Center.x) + roomA.Width / 2 - 1; // ����ǽ�ڵ���� X ����
        int startX2 = Mathf.Max(roomA.Center.x, roomB.Center.x) - roomB.Width / 2;     // ����ǽ�ڵ��յ� X ����
        int groundHeightA = GetGroundHeight(startX1 - 1, roomA.Center.y - roomA.Height / 2, roomA.Height);
        int groundHeightB = GetGroundHeight(startX2 + 1, roomB.Center.y - roomB.Height / 2, roomB.Height);
        int clearHeight = Mathf.Max(groundHeightA, groundHeightB) + 1;

        // ɾ����̺͵�����Ƭ
        spikesLayer[startX1 - 1, groundHeightA + 1] = 0;
        groundLayer[startX1 - 1, groundHeightA + 1] = 0;
        spikesLayer[startX2 + 1, groundHeightB + 1] = 0;
        groundLayer[startX2 + 1, groundHeightB + 1] = 0;

        // ɾ��ǽ�ڲ�����ͨ��
        for (int i = clearHeight; i < clearHeight + doorSize; i++)
        {
            wallLayer[startX1, i] = 0;
            wallLayer[startX2, i] = 0;
            groundLayer[startX1, i] = 0;
            groundLayer[startX2, i] = 0;

            //  Debug.Log($"Deleted wall at: ({startX1}, {i}) and ({startX2}, {i})");
        }

        // �������Ϣ������
        Door newDoorA = new Door(new Vector2Int(startX1, clearHeight), doorSize, true, transform);
        Door newDoorB = new Door(new Vector2Int(startX2, clearHeight), doorSize, true, transform);

        // ToggleDoor(newDoorA, true);
        // ToggleDoor(newDoorA, true);
        roomA.AddDoor(newDoorA, doorTilemap, doorOpenTile);
        roomB.AddDoor(newDoorB, doorTilemap, doorOpenTile);

        //}
        //else if (roomA.Center.x == roomB.Center.x) // ��ֱ��������
        //{
        //int startY1 = Mathf.Min(roomA.Center.y, roomB.Center.y) + roomHeight / 2 - 1; // ����ǽ�ڵ���� Y ����
        //int startY2 = Mathf.Max(roomA.Center.y, roomB.Center.y) - roomHeight / 2;     // ����ǽ�ڵ��յ� Y ����

        //// ɾ��ǽ�ڲ�����ͨ��
        //for (int i = roomA.Center.x - 2; i < roomA.Center.x + 2; i++)
        //{
        //    wallLayer[i, startY1] = 0;
        //    wallLayer[i, startY2] = 0;
        //    Debug.Log($"Deleted wall at: ({i}, {startY1}) and ({i}, {startY2})");
        //}
        //int doorX = roomA.Center.x; // �ŵ� X ����
        //int startY = (roomA.Center.y + roomB.Center.y) / 2 - doorSize / 2; // �ŵ���� Y ����

        //// ���ɶ���Ƭ��
        //for (int i = 0; i < doorSize; i++)
        //{
        //    doorTilemap.SetTile(new Vector3Int(doorX, startY + i, 0), doorOpenTile);
        //}

        // �������Ϣ������
        //Door newDoor = new Door(new Vector2Int(doorX, startY), doorSize, true);
        //roomA.AddDoor(newDoor);
        //roomB.AddDoor(newDoor);
        // }
    }


    void GenerateGround(int startX, int startY, int roomWidth, int roomHeight)
    {
        int lastHeight = startY + (int)(roomHeight * Random.Range(0.1f, 0.8f)); // ��ʼ����߶�

        for (int x = startX; x < startX + roomWidth; x++)
        {
            float noiseValue = Mathf.PerlinNoise((x + seed) * 0.05f, seed * 0.1f);
            int currentHeight = Mathf.FloorToInt(noiseValue * (roomHeight / 4) + lastHeight * 0.5f);

            // ƽ�����Σ����Ƹ߶Ȳ�仯
            currentHeight = Mathf.Clamp(currentHeight, lastHeight - 1, lastHeight + 1);
            currentHeight = Mathf.Clamp(currentHeight, startY + 2, startY + roomHeight - 2);
            lastHeight = currentHeight;

            // ��������Ƭ
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
            wallLayer[x, startY] = 1; // �ײ��߽�
            wallLayer[x, startY + roomHeight - 1] = 1; // �����߽�
        }
        for (int y = startY; y < startY + roomHeight; y++)
        {
            wallLayer[startX, y] = 1; // ��߽�
            wallLayer[startX + roomWidth - 1, y] = 1; // �ұ߽�
        }
        //GameObject Bous = Instantiate(boungs, new Vector3(startX + 0.5f, startY + 0.5f, 0), Quaternion.identity);
        //Bous.GetComponent<TriggER>().SetRoomBounds(roomWidth - 1, roomHeight - 1);
    }

    void GenerateSpikes(int startX, int startY, int roomWidth, int roomHeight)
    {
        for (int x = startX + Random.Range(3, 7); x < startX + roomWidth - 1; x++)
        {
            int groundHeight = GetGroundHeight(x, startY, roomHeight);

            // ȷ�����ֻ�����ڵر�
            if (groundHeight > 0 && groundHeight < startY + roomHeight - 1)
            {
                int spikeLength = Random.Range(3, 6);

                for (int i = 0; i < spikeLength; i++)
                {
                    int spikeX = x + i;

                    if (spikeX < startX + roomWidth - 1 && GetGroundHeight(spikeX, startY, roomHeight) == groundHeight)
                    {
                        spikesLayer[spikeX, groundHeight + 1] = 1; // ��̷����ڵر���
                    }
                }

                x += spikeLength + Random.Range(2, 6); // ������̶β���������
            }
        }
    }

    void GeneratePlatforms(int startX, int startY, int roomWidth, int roomHeight)
    {
        int x = startX + Random.Range(3, 7);

        while (x < startX + roomWidth - 8)
        {
            int platformHeight = Random.Range(GetGroundHeight(x, startY, roomHeight) + 4, startY + roomHeight - 4); // ƽ̨�߶�
            int platformLength = Random.Range(3, 6); // ƽ̨����

            for (int i = 0; i < platformLength; i++)
            {
                if (x + i < startX + roomWidth - 1)
                {
                    platformLayer[x + i, platformHeight] = 1; // ����ƽ̨
                }
            }

            // ����Ƿ���Ҫ����¥��
            int groundHeight = GetGroundHeight(x, startY, roomHeight); // ��ȡ����߶�
            int distanceToGround = platformHeight - groundHeight;

            if (distanceToGround > 8) // ���ƽ̨����泬��8����Ƭ
            {
                if (Random.value < 0.8f) // 80%��������¥��
                {
                    GenerateStairs(x, x + platformLength, platformHeight, startY, roomHeight); // ��������¥�ݵĺ���
                }
            }

            x += platformLength + Random.Range(3, 8); // ������ǰƽ̨�β���������
        }
    }
    void GenerateStairs(int startX, int endX, int platformHeight, int startY, int roomHeight)
    {
        int groundHeight = GetGroundHeight((startX + endX) / 2, startY, roomHeight); // ��ȡ�����е�ĵ���߶�

        // ���� startX �� endX �ļ�̷ֲ�
        List<int> safePositions = new List<int>(); // ��¼��ȫ�������ӵ�λ��
        for (int x = startX; x <= endX; x++)
        {
            int groundHeightAtX = GetGroundHeight(x, startY, roomHeight);
            bool hasSpike = spikesLayer[x, groundHeightAtX + 1] == 1; // ������߶� + 1 �Ƿ��м��

            if (!hasSpike)
            {
                safePositions.Add(x); // ���û�м�̣�����λ�ñ��Ϊ��ȫλ��
            }
        }

        // ���ݼ�̷ֲ����������߼�
        if (safePositions.Count == 0)
        {
            //Debug.Log("�����������ɣ���Χ��ȫ���Ǽ��");
            return; // ȫ���Ǽ�̣���������
        }

        // ���û�м�̣�����ѡ���м�λ������
        int ladderX;
        if (safePositions.Count == (endX - startX + 1))
        {
            ladderX = (startX + endX) / 2; // ��ȫû�м�̣�ѡ���е�
        }
        else
        {
            // ����м�̵��а�ȫ����ѡ��ȫ��������ӽ��е��λ��
            int middleX = (startX + endX) / 2;
            ladderX = safePositions.OrderBy(x => Mathf.Abs(x - middleX)).First(); // �ҵ����е�����İ�ȫλ��
        }

        // ȷ�����Ӵӵ������ɵ�ƽ̨�߶�
        int groundHeightAtLadder = GetGroundHeight(ladderX, startY, roomHeight);
        for (int y = groundHeightAtLadder + 1; y <= platformHeight; y++)
        {
            ladderLayer[ladderX, y] = 1; // ��������
        }

        // Debug.Log($"����������λ�ã�{ladderX}������߶ȣ�{groundHeightAtLadder}��ƽ̨�߶ȣ�{platformHeight}");
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


