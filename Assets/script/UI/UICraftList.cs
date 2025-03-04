using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICraftList : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;
    public EquipmentType equipmentType;

    [SerializeField] private List<ItemDataEquipment> craftEquipment;
    public List<ItemDataEquipment> loadedEquipment;
    public bool isShop;
    //[SerializeField] private List<UICraftSlot> craftSlots;

    // Start is called before the first frame update
    void  Start()
    {
         AssingCraftSlots();
        transform.parent.GetChild(0).GetComponent<UICraftList>().SetupCraftList();
        SetupDefaultCraftWindow();

    }
    private void AssingCraftSlots()
    {
        List<ItemData> itemDatas = Inventory.instance.itemDataBase;
       
        foreach (var itemData in itemDatas)
        {
            ItemDataEquipment itemDataEquipment = itemData as ItemDataEquipment;
            if (itemDataEquipment != null && itemDataEquipment.equipmentType == equipmentType)
            {
                loadedEquipment.Add(itemData as ItemDataEquipment);
            }

        }
        craftEquipment = loadedEquipment;
    }
    public void SetupCraftList()
    {
        /* for (int i = 0; i < craftSlots.Count; i++)
         {
             Destroy(craftSlots[i].gameObject);
         }
         craftSlots= new List<UICraftSlot>();
        */
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < craftEquipment.Count; i++)
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<UICraftSlot>().SetupCraftSlot(craftEquipment[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }
    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment[0] != null)
        {
            //GetComponentsInParent<UI>().craftWindow
            GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
            GetComponentInParent<UI>().craftWindow.gameObject.SetActive(false);
            if (isShop)
            {
                GetComponentInParent<UI>().shopWindow.SetupShopWindow(craftEquipment[0]);
                GetComponentInParent<UI>().shopWindow.gameObject.SetActive(false);
                
            }
        }

    }
}
