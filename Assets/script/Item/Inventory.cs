using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;
    public bool havestartItem;
    public List<ItemData> startingitems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemDataEquipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stashItems;
    public Dictionary<ItemData, InventoryItem> stashItemDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform StatSlotParent;
    private UIItemSlot[] inventoryitemSlots;
    private UIItemSlot[] stashitemSlots;
    private SlotID[] inventoryitemSlotBag;
    private SlotID[] stashitemSlotBag;


    private UIEquipmentSlot[] equipmentSlots;
    private UIStatSlot[] statSlots;

    private int inventorySlotCount;
    private int stashSlotCount;


    [Header("Items cooldown")]
    public float flaskCooldown { get; private set; }
    private float lastFlaskTime;
    private float lastArmorTime;

    private float FalskCooldown;
    private float ArmorCooldown;

    [Header("Data base")]
    //public string[] assetNames;
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemDataEquipment> loadedEquipment;

    private bool isStart;
   


    private void Awake()
    {
        inventorySlotCount = 72;
        stashSlotCount = 175;
        // FillUpItemdateBase();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
#if UNITY_EDITOR
      FillUpItemdateBase();
#endif
    }
    private void Start()
    {
        if (!isStart)
        {    
            inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
            inventory = new List<InventoryItem>();
            stashItemDictionary = new Dictionary<ItemData, InventoryItem>();
            stashItems = new List<InventoryItem>();
            equipmentDictionary = new Dictionary<ItemDataEquipment, InventoryItem>();
            equipment = new List<InventoryItem>();
            //inventoryitemSlots = itemSlotParent.GetComponentsInChildren<UIItemSlot>();
            //stashitemSlots = stashSlotParent.GetComponentsInChildren<UIItemSlot>();
            equipmentSlots = equipmentSlotParent.GetComponentsInChildren<UIEquipmentSlot>();
            statSlots = StatSlotParent.GetComponentsInChildren<UIStatSlot>();
            


            StartSlots();

            //AddStaringItems();
            Invoke("AddStaringItems", 0.2f);
            isStart = true;
        }
        else
        {
            foreach (ItemDataEquipment item in loadedEquipment)
            {
                EquipItem(item);
            }

        }
    }
    private void Update()
    {
        // FillUpItemdateBase();
    }

    void StartSlots()
    {

        for (int i = 0; i < inventorySlotCount; i++)
        {
            GameObject obj = PoolMgr.Instance.GetObj("ItemSlot", transform.position, Quaternion.identity);
                //ProxyResourceFactory.Instance.Factory.GetUI("ItemSlot");
            obj.GetComponent<SlotID>().ID = i + 1;
            obj.GetComponentInChildren<UIItemSlot>().SlotID = i + 1;
            obj.transform.SetParent(itemSlotParent);


        }
        for (int i = 0; i < stashSlotCount; i++)
        {
            GameObject obj = PoolMgr.Instance.GetObj("ItemSlot", transform.position, Quaternion.identity);
                // ProxyResourceFactory.Instance.Factory.GetUI("ItemSlot");
            obj.GetComponent<SlotID>().ID = i + 1;
            obj.GetComponentInChildren<UIItemSlot>().SlotID = i + 1;
            obj.transform.SetParent(stashSlotParent);
        }
        inventoryitemSlots = itemSlotParent.GetComponentsInChildren<UIItemSlot>();
        inventoryitemSlotBag = itemSlotParent.GetComponentsInChildren<SlotID>();
        stashitemSlots = stashSlotParent.GetComponentsInChildren<UIItemSlot>();
        stashitemSlotBag = stashSlotParent.GetComponentsInChildren<SlotID>();
    }
    private void AddStaringItems()
    {
        foreach (ItemDataEquipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
               // Debug.Log(item.slotID);
                for (int i = 0; i < item.stackSize; i++)
                {

                    AddItem(item.data, item.slotID);
                }
            }
            return;
        }

        if (havestartItem)
        {
            for (int i = 0; i < startingitems.Count; i++)
            {
                if (startingitems[i] != null)
                {
                    AddItem(startingitems[i]);
                    //Debug.Log("added item");
                }
            }
        }
    }
    public void EquipItem(ItemData itemData)
    {
        ItemDataEquipment itemDataEquipment, itemToDelete;
        InventoryItem newitem;
        chackEqip(itemData, out itemDataEquipment, out newitem, out itemToDelete);

        if (itemToDelete != null)
        {

            if (!CanequipItem(itemToDelete)) return;
            UnequipItem(itemToDelete);
            AddItem(itemToDelete);


        }
        AudioManager.instance.PlaySFX(25, null);
        equipment.Add(newitem);
        equipmentDictionary.Add(itemDataEquipment, newitem);
        itemDataEquipment.AddModifiers();

        RemoveItem(itemData);


        UpdateSlotUI();



    }

    private void chackEqip(ItemData itemData, out ItemDataEquipment itemDataEquipment, out InventoryItem newitem, out ItemDataEquipment itemToDelete)
    {
        itemDataEquipment = itemData as ItemDataEquipment;
        newitem = new InventoryItem(itemDataEquipment);
        itemToDelete = null;
        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> kvp in equipmentDictionary)
        {
            if (kvp.Key.equipmentType == itemDataEquipment.equipmentType)
            {
                itemToDelete = kvp.Key;
            }
        }
    }
    public ItemDataEquipment GetItemDataEquipment(ItemData itemData)
    {
        if (itemData.itemType == ItemType.Material) return null;

        ItemDataEquipment itemDataEquipment = itemData as ItemDataEquipment;

        ItemDataEquipment itemToDelete = null;

        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> kvp in equipmentDictionary)
        {
            if (kvp.Key.equipmentType == itemDataEquipment.equipmentType)
            {
                itemToDelete = kvp.Key;
            }
        }
        return itemToDelete;
    }

    public void UnequipItem(ItemDataEquipment itemToDelete)
    {
        AudioManager.instance.PlaySFX(28, null);
        if (equipmentDictionary.TryGetValue(itemToDelete, out InventoryItem item))
        {
            equipment.Remove(item);
            equipmentDictionary.Remove(itemToDelete);
            itemToDelete.RemoveModifiers();
        }
    }

    public void AddEquipmentModifiers()
    {
        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> kvp in equipmentDictionary)
        {
            kvp.Key.AddModifiers();

        }
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {


        for (int i = 0; i < inventoryitemSlots.Length; i++)
        {
            inventoryitemSlots[i].CleanSlot();
        }
        for (int i = 0; i < stashitemSlots.Length; i++)
        {
            stashitemSlots[i].CleanSlot();
        }
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].CleanSlot();
        }



       

        for (int i = 0; i < inventory.Count; i++)
        {

            inventoryitemSlots[i].UpdateSlot(inventory[i]);

        }
        for (int i = 0; i < stashItems.Count; i++)
        {
            stashitemSlots[i].UpdateSlot(stashItems[i]);
           
        }
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            foreach (KeyValuePair<ItemDataEquipment, InventoryItem> kvp in equipmentDictionary)
            {
                if (kvp.Key.equipmentType == equipmentSlots[i].equipmentType)
                {
                    equipmentSlots[i].UpdateSlot(kvp.Value);
                    //equipmentSlots[i].itemID = i;
                }
            }
        }
       SlotChange();




        UpdateStatsUI();
    }

    private void SlotChange()
    {
        for (int i = 0; i < inventoryitemSlots.Length; i++)
        {
            if (inventoryitemSlots[i].item != null)
            {
                SlotID slotID = inventoryitemSlots[i].transform.parent.GetComponent<SlotID>();
                if (inventoryitemSlots[i].SlotID != slotID.ID)
                {
                   // Debug.Log("slotID: " + slotID.ID + " inventoryitemSlots[i].SlotID: " + inventoryitemSlots[i].SlotID);
                    for (int j = 0; j < inventoryitemSlotBag.Length; j++)
                    {
                        if (inventoryitemSlots[i].SlotID == inventoryitemSlotBag[j].ID)
                        {
                            UIItemSlot SlotBag = inventoryitemSlotBag[j].transform.GetComponentInChildren<UIItemSlot>();
                            if (SlotBag.item == null)
                            {
                               SlotBag.SlotID = slotID.ID;
                            }

                            SlotBag.transform.SetParent(slotID.transform);
                            SlotBag.transform.position= slotID.transform.position;
                            inventoryitemSlots[i].transform.SetParent(inventoryitemSlotBag[j].transform);
                            inventoryitemSlots[i].transform.position= inventoryitemSlotBag[j].transform.position;

                        }
                    }
                }
            }
           
           

        }
        for (int i = 0; i < stashitemSlots.Length; i++)
        {
           if (stashitemSlots[i].item != null)
            {
                SlotID slotID = stashitemSlots[i].transform.parent.GetComponent<SlotID>();
                if (stashitemSlots[i].SlotID != slotID.ID)
                {
                    for (int j = 0; j < stashitemSlotBag.Length; j++)
                    {
                        if (stashitemSlots[i].SlotID == stashitemSlotBag[j].ID)
                        {
                            UIItemSlot SlotBag = stashitemSlotBag[j].transform.GetComponentInChildren<UIItemSlot>();
                            if (SlotBag.item == null)
                            {
                                SlotBag.SlotID = slotID.ID;
                            }
                            SlotBag.transform.SetParent(slotID.transform);
                            SlotBag.transform.position = slotID.transform.position;
                            stashitemSlots[i].transform.SetParent(stashitemSlotBag[j].transform);
                            stashitemSlots[i].transform.position = stashitemSlotBag[j].transform.position;
                           
                        }
                    }
                }
            }
           
        }
    }

    public void UpdateStatsUI()
    {
        if(stashItems.Count==0) { return; }
        for (int i = 0; i < statSlots.Length; i++)
        {
            statSlots[i].UpdateStatValueUI();
        }
    }
    public void ChangeStatsUI(ItemDataEquipment olditem, ItemDataEquipment newitem)
    {
        for (int i = 0; i < statSlots.Length; i++)
        {
            statSlots[i].changeeStatValueUI(olditem, newitem);
        }
    }

    public void AddItem(ItemData itemData, int slotID = 0)
    {
        if (itemData.itemType == ItemType.Equipment)
        {
            AddToInventory(itemData, slotID);
        }
        if (itemData.itemType == ItemType.Material)
        {
            AddToStash(itemData,slotID);
        }
        UpdateSlotUI();
    }
    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryitemSlots.Length)
        {
            return false;
        }
        return true;
    }
    public bool CanequipItem(ItemData itemData)
    {
        //ItemDataEquipment itemDataEquipment = itemData as ItemDataEquipment;

        if (itemData.itemType == ItemType.Equipment)
        {
            int equippedaddCount = 0;
            if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
            {
                equippedaddCount = 0;
            }
            else
            {
                equippedaddCount = 1;
            }
            if (inventory.Count + equippedaddCount > inventoryitemSlots.Length)
            {
                Debug.Log(inventory.Count + equippedaddCount);
                return false;
            }
        }
        if (itemData.itemType == ItemType.Material)
        {
            int addCount = 0;
            if (stashItemDictionary.TryGetValue(itemData, out InventoryItem item))
            {
                addCount = 0;
            }
            else
            {
                addCount = 1;
            }
            //if (inventory.Count + equippedaddCount > inventoryitemSlots.Length)
            if (stashItems.Count + addCount > stashitemSlots.Length)
            {
                Debug.Log(inventory.Count + addCount);
                return false;
            }
        }
        return true;
    }

    private void AddToInventory(ItemData itemData, int slotID = 0)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            
                item.AddStack();
            
        }
        else if (CanAddItem())
        {
          
            InventoryItem newItem = new InventoryItem(itemData, slotID);

            SlotID itemSlot = GetInventory();
          

            if (itemSlot != null && newItem.slotID == 0)
            {
                newItem.slotID = itemSlot.ID;
            }

            inventory.Add(newItem);
            inventoryDictionary.Add(itemData, newItem);
           // SlotChange();
        }
    }
    private void AddToStash(ItemData itemData, int slotID = 0)
    {
        if (stashItemDictionary.TryGetValue(itemData, out InventoryItem item) )
        {
            item.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(itemData, slotID);
            SlotID itemSlot = Getstash();
            if (itemSlot != null&& newItem.slotID == 0)
            {
                newItem.slotID = itemSlot.ID;
            }
           
            stashItems.Add(newItem);
            stashItemDictionary.Add(itemData, newItem);
            //SlotChange();
        }
    }
    private SlotID GetInventory()
    {
        for (int i = 0; i < inventoryitemSlotBag.Length; i++)
        {
            if (inventoryitemSlotBag[i].transform.GetComponentInChildren<UIItemSlot>().item == null)
            {
                return inventoryitemSlotBag[i];
            }
        }
        return null;
    }

    private SlotID Getstash()
    {
        for (int i = 0; i < stashitemSlotBag.Length; i++)
        {
            if (stashitemSlotBag[i].transform.GetComponentInChildren<UIItemSlot>().item == null)
            {
                return stashitemSlotBag[i];
            }
        }
       

        return null;

    }

    public int GetStashCount(ItemData itemData)
    {
        if (stashItemDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            return item.stackSize;
        }
        return 0;
    }
    public int GetInventoryCount(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            return item.stackSize;
        }
        return 0;
    }

    public void RemoveItem(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            if (item.stackSize <= 1)
            {
                inventory.Remove(item);
                inventoryDictionary.Remove(itemData);
                //SlotChange();
            }
            else
            {
                item.RemoveStack();
            }

        }


        if (stashItemDictionary.TryGetValue(itemData, out InventoryItem stashItem))
        {
            if (stashItem.stackSize <= 1)
            {
                stashItems.Remove(stashItem);
                stashItemDictionary.Remove(itemData);
                //SlotChange();
            }
            else
            {
                stashItem.RemoveStack();
            }
        }




        UpdateSlotUI();
    }

    public bool CanCraft(ItemDataEquipment itemToCraft, List<InventoryItem> requiredMaterials)
    {
        List<InventoryItem> materials = new List<InventoryItem>();

        for (int i = 0; i < requiredMaterials.Count; i++)
        {
            if (stashItemDictionary.TryGetValue(requiredMaterials[i].data, out InventoryItem item))
            {
                if (item.stackSize < requiredMaterials[i].stackSize)
                {
                    AudioManager.instance.PlaySFX(17, null);
                    return false;
                }
                else
                {
                    materials.Add(item);

                }
            }
            else
            {
                AudioManager.instance.PlaySFX(17, null);
                return false;
            }
        }
        AudioManager.instance.PlaySFX(20, null);

        for (int i = 0; i < materials.Count; i++)
        {

            for (int j = 0; j < requiredMaterials[i].stackSize; j++)
            {

                RemoveItem(materials[i].data);

            }
        }
        AddItem(itemToCraft);
        for (int i = 0; i < requiredMaterials.Count; i++)
        {


            UI.instance.craftWindow.materialImage[i].sprite = requiredMaterials[i].data.icon;
            UI.instance.craftWindow.materialImage[i].color = Color.white;
            int havestack = Inventory.instance.GetStashCount(requiredMaterials[i].data);
            int needstack = requiredMaterials[i].stackSize;

            UI.instance.craftWindow.materialImage[i].GetComponentInChildren<TextMeshProUGUI>().text = havestack.ToString() + "/" + needstack.ToString();
            if (havestack >= needstack)
            {
                UI.instance.craftWindow.materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                UI.instance.craftWindow.materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
            UI.instance.craftWindow.materialImage[i].GetComponentInChildren<TextMeshProUGUI>().fontSize = 16;

        }
        UI.instance.craftWindow.itemcounttext.text = "持有数量：" + GetInventoryCount(itemToCraft).ToString();
        return true;
    }
    public void BuyItem(ItemData itemTobuy, int money)
    {
        if (!PlayerManager.instance.HaveEnoughMoney(money)) return;
        AddItem(itemTobuy);
        UI.instance.shopWindow.itemcounttext.text= "持有数量："+ GetInventoryCount(itemTobuy).ToString();
        UI.instance.shopWindow.haveMoneytext.text = "持有金钱：" + PlayerManager.instance.currency.ToString();
    }

    public List<InventoryItem> GetEquipmentList() => equipment;
    public List<InventoryItem> GetStashItemsList() => stashItems;

    public ItemDataEquipment GetEquipmentType(EquipmentType type)
    {
        ItemDataEquipment item = null;
        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> kvp in equipmentDictionary)
        {
            if (kvp.Key.equipmentType == type)
            {
                item = kvp.Key;
            }
        }
        return item;
    }

    public void UseFlask()
    {
        ItemDataEquipment flask = GetEquipmentType(EquipmentType.Flask);
        if (flask == null) return;



        // bool canUse = Time.time > flaskCooldown + lastFlaskTime;
        if (CanUseFlask(flask))
        {
            if (inventoryDictionary.TryGetValue(flask, out InventoryItem item))
            {
                RemoveItem(flask);

            }
            else
            {
                UnequipItem(flask);


            }
            //RemoveItem(flask);
            flaskCooldown = flask.itemCooldown;
            lastFlaskTime = Time.time;
            flask.ItemEffect(null);
            UpdateSlotUI();
        }
    }
    public bool CanUseFlask(ItemData itemData)
    {
        ItemDataEquipment itemDataEquipment, itemToDelete;
        InventoryItem newitem;
        chackEqip(itemData, out itemDataEquipment, out newitem, out itemToDelete);
        if (itemToDelete == null) return false;

        return Time.time > flaskCooldown + lastFlaskTime;

    }
    public int GetFlaskSize()
    {
        ItemDataEquipment flask = GetEquipmentType(EquipmentType.Flask);
        if (flask == null) return 0;
        if (inventoryDictionary.TryGetValue(flask, out InventoryItem item))
        {
            return item.stackSize + 1;
        }
        if (flask != null) return 1;
        return 0;
    }

    public bool CanUseArmor()
    {
        ItemDataEquipment armor = GetEquipmentType(EquipmentType.Armor);
        if (armor == null) return false;
        if (Time.time > ArmorCooldown + lastArmorTime)
        {
            ArmorCooldown = armor.itemCooldown;
            lastArmorTime = Time.time;
            return true;
        }
        return false;
    }

    public void LoadData(GameData data)
    {
       // instance = data.inventorys;



        // List < ItemData > itemDataBase = GetItemDataBase();
        //Debug.Log(itemDataBase.Count);
        //GetItemDataBase();
        
        foreach (KeyValuePair<string, int> kvp in data.inventory)
        {
            // Debug.Log(loadedItems.Count);
            foreach (var itemData in itemDataBase)
            {
                if (itemData != null && itemData.itemID == kvp.Key)
                {
                    InventoryItem inventoryItem = new InventoryItem(itemData);
                    foreach (KeyValuePair<string, int> kvp2 in data.inventorySlot)
                    {
                        if (kvp2.Key == kvp.Key)
                        {
                            inventoryItem.slotID = kvp2.Value;
                            //Debug.Log("slot id " + kvp2.Value);
                        }
                    }
                   
                    inventoryItem.stackSize = kvp.Value;
                    loadedItems.Add(inventoryItem);
                }
            }
       
        
        }
       
        foreach (string itemID in data.equipmentID)
        {
            foreach (var itemData in itemDataBase)
            {
                if (itemData != null && itemData.itemID == itemID)
                {
                    loadedEquipment.Add(itemData as ItemDataEquipment);
                }
            }
        }
         
    }

    public void SaveData(ref GameData data)
    {
       //data.inventorys = instance;
        
        data.inventory.Clear();
        data.inventorySlot.Clear();
        data.equipmentID.Clear();
        foreach (KeyValuePair<ItemData, InventoryItem> kvp in inventoryDictionary)
        {
            data.inventory.Add(kvp.Key.itemID, kvp.Value.stackSize);
            
        }
        foreach (KeyValuePair<ItemData, InventoryItem> kvp in inventoryDictionary)
        {
            data.inventorySlot.Add(kvp.Key.itemID, kvp.Value.slotID);
            //Debug.Log("slot id " + kvp.Value.slotID);
        }
        foreach (KeyValuePair<ItemData, InventoryItem> kvp in stashItemDictionary)
        {
            data.inventory.Add(kvp.Key.itemID, kvp.Value.stackSize);
            
        }
        foreach (KeyValuePair<ItemData, InventoryItem> kvp in stashItemDictionary)
        {
            data.inventorySlot.Add(kvp.Key.itemID, kvp.Value.slotID);
            //Debug.Log("slot id " + kvp.Value.slotID);
        }
        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> kvp in equipmentDictionary)
        {
            data.equipmentID.Add(kvp.Key.itemID);
        }
        
    }


#if UNITY_EDITOR
    [ContextMenu("Fill Up ItemDataBase")]
    private void FillUpItemdateBase() => itemDataBase = new List<ItemData>(GetItemDataBase());
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Resources_moved/Data/Datas" });
        //Equipment
        foreach (string assetName in assetNames)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (itemData != null)
            itemDataBase.Add(itemData);
        }
        return itemDataBase;
    }
#endif
}
