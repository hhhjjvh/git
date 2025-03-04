using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICraftSlot : UIItemSlot
{
    public bool isShop;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
   
    private void OnEnable()
    {
        UpdateSlot(item);
    }
    public void SetupCraftSlot(ItemDataEquipment data)
    {
        if (data == null) return;
        item.data = data;
        itemImage.sprite = data.icon;
        itemAmountText.text = data.itemName;

        if(itemAmountText.text.Length > 12)
        {
            itemAmountText.fontSize *= 0.7f;
        }
        else
        {
            itemAmountText.fontSize = 24;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //ItemDataEquipment  craftData=item.data as ItemDataEquipment;
        //Inventory.instance.CanCraft(craftData, craftData.craftRequirements);

       
        if(isShop)
        {
            ui.shopWindow.SetupShopWindow(item.data as ItemDataEquipment);
        }
        else
        {
            ui.craftWindow.SetupCraftWindow(item.data as ItemDataEquipment);
        }

    }
}
