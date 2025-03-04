using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ShopRoom : Room
{

    public ShopRoom(int width, int height, Vector2Int center, int difficulty) : base(width, height, center, RoomType.ShopRoom, difficulty) 
    { 
        GenerateItems();
    }

    public List<ItemData> availableItems = new List<ItemData>();  // �ɹ������Ʒ
   
    

    // �����̵��ѶȻ����������Ʒ
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

        int itemCount = Random.Range(3, 6); // ���ѡ����Ʒ����
        for (int i = 0; i < itemCount; i++)
        {
            ItemData randomItem = items[Random.Range(0, items.Count)];
            availableItems.Add(randomItem);
        }

        //UpdateShopUI();
    }

    //// �����̵�UI
    //private void UpdateShopUI()
    //{
        
    //    // ��ʾ�̵��е���Ʒ
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
    //// ���ɵ�����Ծƽ̨
    //private void GenerateJumpPlatforms()
    //{
    //    int platformCount = availableItems.Count; // ÿ����Ʒ��Ӧһ��ƽ̨
    //    float platformSpacing = (Width - 6) / (float)platformCount;  // ���ݷ����Ⱥ���Ʒ�������ü��

    //    for (int i = 0; i < platformCount; i++)
    //    {
    //        float platformX = Center.x - Width / 2 + 3 + i * platformSpacing;
    //        Vector2 platformPosition = new Vector2(platformX, Center.y + 4); // ����� 4 ��

    //        // ���ɵ�����Ծƽ̨
    //        //GameObject platform = Instantiate(jumpPlatformPrefab, platformPosition, Quaternion.identity);
    //       // platform.transform.SetParent(parent);

    //        // Ϊƽ̨�Ϸ���һ����Ʒ
    //        ItemData item = availableItems[i];
    //        //SpawnItemOnPlatform(platform, item);
    //    }
    //}

    //// ��ƽ̨�Ϸ�����Ʒ
    //private void SpawnItemOnPlatform(GameObject platform, ItemData item)
    //{
    //    Vector2 platformCenter = platform.transform.position;
    //    Vector2 itemPosition = new Vector2(platformCenter.x, platformCenter.y + 0.5f);  // ������ƽ̨�м�

    //    // ��ȡ��Ʒ����������λ��
    //    GameObject itemObj = PoolMgr.Instance.GetObj("Item _Sword", itemPosition);
    //    itemObj.GetComponent<ItemObject>().SetUpItem(item, new Vector2(0, 5f), this);
    //    itemObj.transform.SetParent(parent);
    //}

    // ���Թ�����Ʒ
    public bool TryBuyItem(ItemData item)
    {
        return PlayerManager.instance.HaveEnoughMoney(item.needMoney);
       
    }
    public void GenerateShopPlatforms()
    {
        RandomMapGenerator.Instance.GenerateJumpPlatforms(this, availableItems);
    }

}

