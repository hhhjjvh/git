using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreasureRoom : Room
{
    
    public TreasureRoom(int width, int height, Vector2Int center,int difficulty) : base(width, height, center, RoomType.TreasureRoom,difficulty) 
    { 
        
       // SetChest();
        
    }

    // 打开宝箱并随机奖励
    public void SetChest()
    {
        DataBase = Inventory.instance.itemDataBase;
        foreach (var itemData in DataBase)
        {
            if (itemData != null && itemData.itemType == ItemType.Equipment&&itemData.needMoney<=(int)(Difficulty* 3000))
            {
                items.Add(itemData);
            }
        }
       
        Vector2 roomStart = new Vector2(Center.x - Width / 2 + 2, Center.y);
        Vector2 roomEnd = new Vector2(Center.x + Width / 2, Center.y + Height / 2-2);
        Vector2 spawnPosition = new Vector2(
              Random.Range(roomStart.x + 1, roomEnd.x - 1),
              Random.Range(roomStart.y + 1, roomEnd.y - 1)
          );
        GameObject Chest = PoolMgr.Instance.GetObj("BrownTreasureBox", spawnPosition);
        Chest.GetComponentInChildren<BrownTreasureBox>().SetDropItems(items);
        Chest.name = "Chest";
        Chest.transform.SetParent(parent);
    }
    public override void InitializeWaveSystem(GameObject prefab, Transform parentTransform)
    {
        base.InitializeWaveSystem(prefab, parentTransform);
        SetChest();
    }



}

