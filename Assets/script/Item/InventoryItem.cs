using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItem 
{
    public ItemData data;
    public int stackSize;
    public int slotID;
    public InventoryItem(ItemData data, int slotID = 0)
    {
        this.data = data;
        this.slotID = slotID;
        AddStack();
    }

    public void AddStack()=> stackSize++;
    public void RemoveStack()=> stackSize--;

    public void SetSlotID(int id)=> slotID = id;


}

