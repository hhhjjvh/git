using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ShopRoom : Room
{

    public ShopRoom(int width, int height, Vector2Int center, int difficulty) : base(width, height, center, RoomType.ShopRoom, difficulty) 
    { 
        GenerateItems();
    }

    public List<ItemData> availableItems = new List<ItemData>();  // 可购买的物品
   
    

    // 根据商店难度或随机生成物品
    public void GenerateItems()
    {
        availableItems.Clear();
        DataBase = Inventory.instance.itemDataBase;
        foreach (var itemData in DataBase)
        {
            if (itemData != null && itemData.itemType == ItemType.Equipment&& itemData.needMoney <= (int)(Difficulty * 50000))
            {
                items.Add(itemData);
            }
        }

        int itemCount = Random.Range(3, 6); // 随机选择物品数量
        for (int i = 0; i < itemCount; i++)
        {
            ItemData randomItem = items[Random.Range(0, items.Count)];
            availableItems.Add(randomItem);
        }

        //UpdateShopUI();
    }

    //// 更新商店UI
    //private void UpdateShopUI()
    //{
        
    //    // 显示商店中的物品
    //    foreach (ItemData item in availableItems)
    //    {
    //        Vector2 roomStart = new Vector2(Center.x - Width / 2 + 2, Center.y);
    //        Vector2 roomEnd = new Vector2(Center.x + Width / 2, Center.y + Height / 2);
    //        Vector2 spawnPosition = new Vector2(
    //              Random.Range(roomStart.x + 1, roomEnd.x - 1),
    //              Random.Range(roomStart.y + 1, roomEnd.y - 1)
    //          );
    //        GameObject weapon = PoolMgr.Instance.GetObj("Item _Sword", spawnPosition);
    //        Vector2 randomDirection = new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f));
    //        weapon.GetComponent<ItemObject>().SetUpItem(item, randomDirection,this);
    //        weapon.transform.SetParent(parent);
    //    }
    //}
    //// 生成单向跳跃平台
    //private void GenerateJumpPlatforms()
    //{
    //    int platformCount = availableItems.Count; // 每个物品对应一个平台
    //    float platformSpacing = (Width - 6) / (float)platformCount;  // 根据房间宽度和物品数量设置间隔

    //    for (int i = 0; i < platformCount; i++)
    //    {
    //        float platformX = Center.x - Width / 2 + 3 + i * platformSpacing;
    //        Vector2 platformPosition = new Vector2(platformX, Center.y + 4); // 离地面 4 格

    //        // 生成单向跳跃平台
    //        //GameObject platform = Instantiate(jumpPlatformPrefab, platformPosition, Quaternion.identity);
    //       // platform.transform.SetParent(parent);

    //        // 为平台上放置一个物品
    //        ItemData item = availableItems[i];
    //        //SpawnItemOnPlatform(platform, item);
    //    }
    //}

    //// 在平台上放置物品
    //private void SpawnItemOnPlatform(GameObject platform, ItemData item)
    //{
    //    Vector2 platformCenter = platform.transform.position;
    //    Vector2 itemPosition = new Vector2(platformCenter.x, platformCenter.y + 0.5f);  // 放置在平台中间

    //    // 获取物品并设置它的位置
    //    GameObject itemObj = PoolMgr.Instance.GetObj("Item _Sword", itemPosition);
    //    itemObj.GetComponent<ItemObject>().SetUpItem(item, new Vector2(0, 5f), this);
    //    itemObj.transform.SetParent(parent);
    //}

    // 尝试购买物品
    public bool TryBuyItem(ItemData item)
    {
        return PlayerManager.instance.HaveEnoughMoney(item.needMoney);
       
    }
    public void GenerateShopPlatforms()
    {
        RandomMapGenerator.Instance.GenerateJumpPlatforms(this, availableItems);
    }

}

