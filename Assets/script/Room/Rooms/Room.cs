using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

//using static UnityEngine.RuleTile.TilingRuleOutput;
public enum RoomType
{
    StartRoom,  // 初始房间
    NormalRoom, // 普通房间
    BossRoom,   // Boss房间
    TreasureRoom, // 宝箱房
    ShopRoom// 商店房

}
public class Room
{
    public int Width { get;  set; }                // 房间宽度
    public int Height { get;  set; }               // 房间高度
    public Vector2Int Center { get;  set; }        // 房间中心点
    public List<Door> Doors { get; private set; }         // 房间的门列表
    public List<GameObject> Enemies { get; private set; } // 房间中的敌人列表
    public bool IsCleared { get; private set; } = false;  // 房间是否已清空
    public bool isEnemySpawned { get; set; } = false; // 房间是否已生成敌人
    public RoomType Type { get; private set; }           // 房间类型
    public float DistanceFromStart { get; set; } // 与初始房间的距离
    public int totalWaves;         // 总波次数
    public int currentWave;        // 当前波次
    public int EnemyCount;
    public int Difficulty { get; set; }  // 新增 Difficulty 属性，用于控制强度


    private GameObject enemyPrefab; // 敌人预制体
    protected Transform parent;       // 父级对象（用于组织敌人）
    GameObject triggerObject;
    public BossUIManager bossUIManager; // Boss UI 管理器
    public EnemyUIManager enemyUIManager;

    protected List<ItemData> items = new List<ItemData>();
    protected List<ItemData> DataBase = new List<ItemData>();

    GameObject Bossenemy;
    public Room(int width, int height, Vector2Int center, RoomType type,int difficulty)
    {
        parent= GameObject.Find("RoomSet").transform;
        Width = width;
        Height = height;
        Center = center;
        Doors = new List<Door>();
        Enemies = new List<GameObject>();
        Type = type;
        bossUIManager = GameObject.Find("Manager").GetComponent<BossUIManager>();
        Difficulty = difficulty; // 初始化 Difficulty 属性
        enemyUIManager = EnemyUIManager.Instance; // 获取 EnemyUIManager 实例
        enemyUIManager.UpdateDifficulty(Difficulty); // 更新难度等级
        if(type == RoomType.StartRoom)
        {
            GameObject startRoomNPC= null;
           int random = Random.Range(0, 10);
            if (random < 3)
            {
                Vector3 position = new Vector3(Center.x + Width / 2 - Random.Range(7, 12), Center.y - Height / 2 + 5, 0);
                startRoomNPC = PoolMgr.Instance.GetObj("shop", position);
            }
            else if (random < 6) 
            {
                Vector3 position = new Vector3(Center.x + Width / 2 - Random.Range(3,8), Center.y - Height / 2 + 2, 0);
                startRoomNPC = PoolMgr.Instance.GetObj("Alchemist", position);
            }
            if (startRoomNPC != null)
            {
                startRoomNPC.transform.SetParent(parent);
            }
        }
        if (type == RoomType.BossRoom)
        {
            DataBase = Inventory.instance.itemDataBase;
            foreach (var itemData in DataBase)
            {
                if (itemData != null && itemData.itemType == ItemType.Equipment && itemData.needMoney >= (int)(Difficulty * 1000))
                {
                    items.Add(itemData);
                }
            }
        }
    }
    public void CreateRoomTrigger()
    {
        GameObject trigger = PoolMgr.Instance.GetObj("Bounds");
        // 从对象池获取触发器对象
        triggerObject = trigger;
        triggerObject.transform.position = new Vector3(Center.x - Width / 2 , Center.y - Height / 2 , 0);
        triggerObject.GetComponent<TriggER>().SetRoomBounds(Width, Height);
        triggerObject.transform.SetParent(parent);
    }

    public void ReleaseRoomTrigger()
    {

        // 回收触发器到对象池
        //pool.Release(triggerObject);
        PoolMgr.Instance.Release(triggerObject);
    }
    public void SetType(RoomType type)
    {
        Type = type;
    }
    // 添加门
    public void AddDoor(Door door, Tilemap doorTilemap, Tile doorTile)
    {
        Doors.Add(door);
        SortDoors();
        for (int i = 0; i < (door.IsVertical ? door.Height : door.Width); i++)
        {
            Vector3Int tilePosition = door.IsVertical
                ? new Vector3Int(door.Position.x, door.Position.y + i, 0)
                : new Vector3Int(door.Position.x + i, door.Position.y, 0);

            doorTilemap.SetTile(tilePosition, doorTile); // 开启门
        }
        door.SetDoorState(true);
    }

    // 排序门（按位置）
    private void SortDoors()
    {
        Doors.Sort((a, b) => a.Position.x != b.Position.x ? a.Position.x.CompareTo(b.Position.x) : a.Position.y.CompareTo(b.Position.y));
    }

    // 添加敌人
    public void AddEnemy(GameObject enemy)
    {
        Enemies.Add(enemy);
    }
    public void AddChestNorm()
    {
        Vector2 roomStart = new Vector2(Center.x - Width / 2 + 2, Center.y);
        Vector2 roomEnd = new Vector2(Center.x + Width / 2, Center.y + Height / 2-2);
        Vector2 spawnPosition = new Vector2(
              Random.Range(roomStart.x + 1, roomEnd.x - 1),
              Random.Range(roomStart.y + 1, roomEnd.y - 1)
          );
        GameObject Chest = PoolMgr.Instance.GetObj("WhiteTreasureBox", spawnPosition);
        Chest.name = "Chest";
        Chest.transform.SetParent(parent);
    }
    public void AddChestBoss()
    {
        Vector2 roomStart = new Vector2(Center.x - Width / 2 + 2, Center.y);
        Vector2 roomEnd = new Vector2(Center.x + Width / 2, Center.y + Height / 2-2);
        Vector2 spawnPosition = new Vector2(
              Random.Range(roomStart.x + 1, roomEnd.x - 1),
              Random.Range(roomStart.y + 1, roomEnd.y - 1)
          );
        GameObject Chest = PoolMgr.Instance.GetObj("GoldTreasureBox", spawnPosition);
        Chest.GetComponentInChildren<ITreasureBox>().SetDropItems(items);
        Chest.name = "Chest";
        Chest.transform.SetParent(parent);
    }
    public virtual void InitializeWaveSystem(GameObject prefab, Transform parentTransform)
    {
        if (Type == RoomType.NormalRoom || Type == RoomType.BossRoom)
        {
            enemyPrefab = prefab;
            parent = parentTransform;
            totalWaves = Type == RoomType.BossRoom ? 1 : Random.Range(2, 4); // Boss房间生成一波，普通房间生成多波
            currentWave = 0;
        }
    }

    public void SpawnNextWave(Vector2 roomStart, Vector2 roomEnd)
    {
        if (Type != RoomType.NormalRoom && Type != RoomType.BossRoom) return;
        if (parent==null) return;
        if (currentWave >= totalWaves) return;
        EnemyName[] enemyNames = (EnemyName[])System.Enum.GetValues(typeof(EnemyName));
        int spawnCount = Type == RoomType.BossRoom ? 1 : Random.Range(3, 5); // Boss房间生成一名 Boss
        enemyUIManager.UpdateWaveInfo(currentWave, totalWaves); // 更新波数信息
        enemyUIManager.UpdateEnemyCount(spawnCount, spawnCount); // 更新敌人数
        EnemyCount = spawnCount;

        if (Type == RoomType.BossRoom)
        {
            enemyUIManager.gameObject.SetActive(false);
            Vector2 spawnPosition = new Vector2(
               Random.Range(roomStart.x + 1, roomEnd.x - 1),
               Random.Range(roomStart.y + 1, roomEnd.y - 1)
           );
            EnemyName randomEnemyName = enemyNames[0];
            // GameObject enemy = Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, parent);
            GameObject enemy = EnemyFactory.Instance.GetEnemy(randomEnemyName,Random.Range(Difficulty, Difficulty+5), spawnPosition); // 设置level为1，位置为生成位置
            enemy.transform.Rotate(0, 180, 0);
            enemy.transform.SetParent(parent);//设置父类
            Enemies.Add(enemy);
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            bossUIManager.SetBossUI(enemy.name.ToString(), null, enemyStats.GetMaxHealth(), enemyStats.GetLevel(),enemyStats);
            Bossenemy = enemy;
            enemy.GetComponent<Enemy>().healthBarUI.gameObject.SetActive(false);
        }
        else
        {
           enemyUIManager.gameObject.SetActive(true);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    Random.Range(roomStart.x + 1, roomEnd.x - 1),
                    Random.Range(roomStart.y + 1, roomEnd.y - 1)
                );
                EnemyName randomEnemyName = enemyNames[Random.Range(1, enemyNames.Length)];
                // GameObject enemy = Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, parent);
                GameObject enemy = EnemyFactory.Instance.GetEnemy(randomEnemyName, Random.Range(Difficulty,Difficulty+5), spawnPosition); // 设置level为1，位置为生成位置
                enemy.transform.SetParent(parent);//设置父类
                Enemies.Add(enemy);
            }
        }

        currentWave++;
    }

    public void UpdateBossHealth()
    {
        if (Bossenemy != null&& bossUIManager != null)
        {
            bossUIManager.UpdateHealth(Bossenemy.GetComponent<CharacterStats>().health);
        }
    }
    public bool AreAllWavesCompleted()
    {
        return currentWave >= totalWaves && AreAllEnemiesDefeated();
    }
   

    // 检测是否所有敌人已被消灭
    public bool AreAllEnemiesDefeated()
    {
        Enemies.RemoveAll(enemy => enemy == null || enemy.GetComponent<CharacterStats>().isDead == true
        ||enemy.gameObject.activeSelf==false||enemy.GetComponent<Enemy>().attackLayerName!="Player");
        enemyUIManager.UpdateEnemyCount(Enemies.Count, EnemyCount); // 假设每波有3个敌人
        if (Enemies.Count == 0 && currentWave >= totalWaves)
        {
            MarkCleared();
            enemyUIManager.ShowClearText(); // 显示“全部清除”
        }
       
        //||enemy.GetComponent<Enemy>().attackLayerName!="Player"); // 移除已被销毁的敌人
        return Enemies.Count == 0;
    }

    // 标记房间为已清空
    public void MarkCleared()
    {
        IsCleared = true;
    }

    // 关闭所有门
    
    public IEnumerator CloseAllDoor(Tilemap doorTilemap, Tile doorTile)
    {
        foreach (var door in Doors)
        {
            // 逐渐从下往上关闭门
            if (door.IsVertical)
            {
                for (int i = door.Height - 1; i >= 0; i--)
                {
                    Vector3Int tilePosition = new Vector3Int(door.Position.x, door.Position.y + i, 0);
                    doorTilemap.SetTile(tilePosition, doorTile); // 设置瓦片

                    // 延时
                    yield return new WaitForSeconds(0.2f); // 设置替换瓦片的时间间隔
                }
            }
            else
            {
                for (int i = door.Width - 1; i >= 0; i--)
                {
                    Vector3Int tilePosition = new Vector3Int(door.Position.x + i, door.Position.y, 0);
                    doorTilemap.SetTile(tilePosition, doorTile); // 设置瓦片

                    // 延时
                    yield return new WaitForSeconds(0.1f); // 设置替换瓦片的时间间隔
                }
            }
          // Debug.Log("关闭门");
            // 禁用门的碰撞器
            door.SetDoorState(false);
        }
    }

    public IEnumerator OpenAllDoor(Tilemap doorTilemap, Tile doorTile)
    {
       
        foreach (var door in Doors)
        {
           
            // 逐渐从上往下打开门
            if (door.IsVertical)
            {
                for (int i = 0; i < door.Height; i++)
                {
                    Vector3Int tilePosition = new Vector3Int(door.Position.x, door.Position.y + i, 0);
                    doorTilemap.SetTile(tilePosition, doorTile); // 设置瓦片

                    // 延时
                    yield return new WaitForSeconds(0.2f); // 设置替换瓦片的时间间隔
                }
            }
            else
            {
                for (int i = 0; i < door.Width; i++)
                {
                    Vector3Int tilePosition = new Vector3Int(door.Position.x + i, door.Position.y, 0);
                    doorTilemap.SetTile(tilePosition, doorTile); // 设置瓦片

                    // 延时
                    yield return new WaitForSeconds(0.1f); // 设置替换瓦片的时间间隔
                }
            }
            //Debug.Log("开启门");
            // 启用门的碰撞器
            door.SetDoorState(true);
        }
    }
}

