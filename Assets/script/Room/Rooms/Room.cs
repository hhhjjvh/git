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
    StartRoom,  // ��ʼ����
    NormalRoom, // ��ͨ����
    BossRoom,   // Boss����
    TreasureRoom, // ���䷿
    ShopRoom// �̵귿

}
public class Room
{
    public int Width { get;  set; }                // ������
    public int Height { get;  set; }               // ����߶�
    public Vector2Int Center { get;  set; }        // �������ĵ�
    public List<Door> Doors { get; private set; }         // ��������б�
    public List<GameObject> Enemies { get; private set; } // �����еĵ����б�
    public bool IsCleared { get; private set; } = false;  // �����Ƿ������
    public bool isEnemySpawned { get; set; } = false; // �����Ƿ������ɵ���
    public RoomType Type { get; private set; }           // ��������
    public float DistanceFromStart { get; set; } // ���ʼ����ľ���
    public int totalWaves;         // �ܲ�����
    public int currentWave;        // ��ǰ����
    public int EnemyCount;
    public int Difficulty { get; set; }  // ���� Difficulty ���ԣ����ڿ���ǿ��


    private GameObject enemyPrefab; // ����Ԥ����
    protected Transform parent;       // ��������������֯���ˣ�
    GameObject triggerObject;
    public BossUIManager bossUIManager; // Boss UI ������
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
        Difficulty = difficulty; // ��ʼ�� Difficulty ����
        enemyUIManager = EnemyUIManager.Instance; // ��ȡ EnemyUIManager ʵ��
        enemyUIManager.UpdateDifficulty(Difficulty); // �����Ѷȵȼ�
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
        // �Ӷ���ػ�ȡ����������
        triggerObject = trigger;
        triggerObject.transform.position = new Vector3(Center.x - Width / 2 , Center.y - Height / 2 , 0);
        triggerObject.GetComponent<TriggER>().SetRoomBounds(Width, Height);
        triggerObject.transform.SetParent(parent);
    }

    public void ReleaseRoomTrigger()
    {

        // ���մ������������
        //pool.Release(triggerObject);
        PoolMgr.Instance.Release(triggerObject);
    }
    public void SetType(RoomType type)
    {
        Type = type;
    }
    // �����
    public void AddDoor(Door door, Tilemap doorTilemap, Tile doorTile)
    {
        Doors.Add(door);
        SortDoors();
        for (int i = 0; i < (door.IsVertical ? door.Height : door.Width); i++)
        {
            Vector3Int tilePosition = door.IsVertical
                ? new Vector3Int(door.Position.x, door.Position.y + i, 0)
                : new Vector3Int(door.Position.x + i, door.Position.y, 0);

            doorTilemap.SetTile(tilePosition, doorTile); // ������
        }
        door.SetDoorState(true);
    }

    // �����ţ���λ�ã�
    private void SortDoors()
    {
        Doors.Sort((a, b) => a.Position.x != b.Position.x ? a.Position.x.CompareTo(b.Position.x) : a.Position.y.CompareTo(b.Position.y));
    }

    // ��ӵ���
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
            totalWaves = Type == RoomType.BossRoom ? 1 : Random.Range(2, 4); // Boss��������һ������ͨ�������ɶನ
            currentWave = 0;
        }
    }

    public void SpawnNextWave(Vector2 roomStart, Vector2 roomEnd)
    {
        if (Type != RoomType.NormalRoom && Type != RoomType.BossRoom) return;
        if (parent==null) return;
        if (currentWave >= totalWaves) return;
        EnemyName[] enemyNames = (EnemyName[])System.Enum.GetValues(typeof(EnemyName));
        int spawnCount = Type == RoomType.BossRoom ? 1 : Random.Range(3, 5); // Boss��������һ�� Boss
        enemyUIManager.UpdateWaveInfo(currentWave, totalWaves); // ���²�����Ϣ
        enemyUIManager.UpdateEnemyCount(spawnCount, spawnCount); // ���µ�����
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
            GameObject enemy = EnemyFactory.Instance.GetEnemy(randomEnemyName,Random.Range(Difficulty, Difficulty+5), spawnPosition); // ����levelΪ1��λ��Ϊ����λ��
            enemy.transform.Rotate(0, 180, 0);
            enemy.transform.SetParent(parent);//���ø���
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
                GameObject enemy = EnemyFactory.Instance.GetEnemy(randomEnemyName, Random.Range(Difficulty,Difficulty+5), spawnPosition); // ����levelΪ1��λ��Ϊ����λ��
                enemy.transform.SetParent(parent);//���ø���
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
   

    // ����Ƿ����е����ѱ�����
    public bool AreAllEnemiesDefeated()
    {
        Enemies.RemoveAll(enemy => enemy == null || enemy.GetComponent<CharacterStats>().isDead == true
        ||enemy.gameObject.activeSelf==false||enemy.GetComponent<Enemy>().attackLayerName!="Player");
        enemyUIManager.UpdateEnemyCount(Enemies.Count, EnemyCount); // ����ÿ����3������
        if (Enemies.Count == 0 && currentWave >= totalWaves)
        {
            MarkCleared();
            enemyUIManager.ShowClearText(); // ��ʾ��ȫ�������
        }
       
        //||enemy.GetComponent<Enemy>().attackLayerName!="Player"); // �Ƴ��ѱ����ٵĵ���
        return Enemies.Count == 0;
    }

    // ��Ƿ���Ϊ�����
    public void MarkCleared()
    {
        IsCleared = true;
    }

    // �ر�������
    
    public IEnumerator CloseAllDoor(Tilemap doorTilemap, Tile doorTile)
    {
        foreach (var door in Doors)
        {
            // �𽥴������Ϲر���
            if (door.IsVertical)
            {
                for (int i = door.Height - 1; i >= 0; i--)
                {
                    Vector3Int tilePosition = new Vector3Int(door.Position.x, door.Position.y + i, 0);
                    doorTilemap.SetTile(tilePosition, doorTile); // ������Ƭ

                    // ��ʱ
                    yield return new WaitForSeconds(0.2f); // �����滻��Ƭ��ʱ����
                }
            }
            else
            {
                for (int i = door.Width - 1; i >= 0; i--)
                {
                    Vector3Int tilePosition = new Vector3Int(door.Position.x + i, door.Position.y, 0);
                    doorTilemap.SetTile(tilePosition, doorTile); // ������Ƭ

                    // ��ʱ
                    yield return new WaitForSeconds(0.1f); // �����滻��Ƭ��ʱ����
                }
            }
          // Debug.Log("�ر���");
            // �����ŵ���ײ��
            door.SetDoorState(false);
        }
    }

    public IEnumerator OpenAllDoor(Tilemap doorTilemap, Tile doorTile)
    {
       
        foreach (var door in Doors)
        {
           
            // �𽥴������´���
            if (door.IsVertical)
            {
                for (int i = 0; i < door.Height; i++)
                {
                    Vector3Int tilePosition = new Vector3Int(door.Position.x, door.Position.y + i, 0);
                    doorTilemap.SetTile(tilePosition, doorTile); // ������Ƭ

                    // ��ʱ
                    yield return new WaitForSeconds(0.2f); // �����滻��Ƭ��ʱ����
                }
            }
            else
            {
                for (int i = 0; i < door.Width; i++)
                {
                    Vector3Int tilePosition = new Vector3Int(door.Position.x + i, door.Position.y, 0);
                    doorTilemap.SetTile(tilePosition, doorTile); // ������Ƭ

                    // ��ʱ
                    yield return new WaitForSeconds(0.1f); // �����滻��Ƭ��ʱ����
                }
            }
            //Debug.Log("������");
            // �����ŵ���ײ��
            door.SetDoorState(true);
        }
    }
}

