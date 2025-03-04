using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEquipmentSlot : UIItemSlot
{
    public EquipmentType equipmentType;

    private void OnValidate()
    {
        gameObject.name = "EquipmentSlot (" + equipmentType + ")";
    }
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        
        if (item == null || item.stackSize == 0) return;
        // base.OnPointerDown(eventData);
        if (!Inventory.instance.CanequipItem(item.data as ItemDataEquipment)) return;
        Inventory.instance.UnequipItem(item.data as ItemDataEquipment);
        Inventory.instance.AddItem(item.data as ItemDataEquipment);
       // Debug.Log("Unequip item");
        ui.itemTooltip.HideToolTip();

        CleanSlot();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) return;
        ui.itemTooltip.ShowToolTip(item.data as ItemDataEquipment);
       // ItemDataEquipment oldEquipment = Inventory.instance.GetItemDataEquipment(item.data);
        Inventory.instance.ChangeStatsUI( item.data as ItemDataEquipment, null);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        
    }
    public override void OnEndDrag(PointerEventData eventData)
    {

    }
    public override void OnDrag(PointerEventData eventData)
    {
        
    }
}
