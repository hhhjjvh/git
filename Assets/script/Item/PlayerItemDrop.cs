using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player Item Drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDropItems()
    {
        //base.GenerateDropItems(); 
        List<InventoryItem> currentEquipment = Inventory.instance.GetEquipmentList();
        List<InventoryItem> currentStash = Inventory.instance.GetStashItemsList();
        List<InventoryItem> itemsToDrop = new List<InventoryItem>();
        List<InventoryItem> ToLooseMaterials= new List<InventoryItem>();

        foreach (InventoryItem item in currentEquipment)
        {
            if (Random.Range(0f, 100f) < chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToDrop.Add(item);
                //Inventory.instance.UnequipItem(item.data as ItemDataEquipment);
            }
        }
        foreach (InventoryItem item in itemsToDrop)
        {
            Inventory.instance.UnequipItem(item.data as ItemDataEquipment);
        }
        foreach (InventoryItem item in currentStash)
        {
            if (Random.Range(0f, 100f) < chanceToLooseMaterials)
            {

                DropItem(item.data);
            }
        }
        foreach (InventoryItem item in ToLooseMaterials)
        {
            Inventory.instance.RemoveItem(item.data);
        }

    }
    public void DropItems(ItemData itemData, int facingDirection)
    {
        GameObject dropItem = Instantiate(dropPrefab, transform.position+new Vector3(1*facingDirection,0), Quaternion.identity);
        Vector2 randomDirection = new Vector2(Random.Range(10f, 15f)* facingDirection, Random.Range(5f, 8f));
        dropItem.GetComponent<ItemObject>().SetUpItem(itemData, randomDirection);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
