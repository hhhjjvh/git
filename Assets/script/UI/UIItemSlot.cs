using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemAmountText;

    protected UI ui;
    public InventoryItem item;
    public int SlotID;

    private Transform originalParent;
    //public int itemID;
    private void OnValidate()
    {
        gameObject.name = "Slot";
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
        //UpdateSlot();

    }

    public void UpdateSlot(InventoryItem _item)
    {
        this.item = _item;
        if (item.slotID != 0)
        {
            SlotID = item.slotID;
        }

        itemImage.color = Color.white;
        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemAmountText.text = item.stackSize.ToString();
            }
            else
            {
                itemAmountText.text = "";
            }
        }
    }
    public void CleanSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemAmountText.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0) return;
        AudioManager.instance.PlaySFX(16, null);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            PlayerManager.instance.player.itemDrop.DropItems(item.data, PlayerManager.instance.player.facingDirection);
            Inventory.instance.RemoveItem(item.data);
            return;
        }
        //throw new System.NotImplementedException();
        if (item.data.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(item.data);
        }
        ui.itemTooltip.HideToolTip();


    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0) return;
        ui.itemTooltip.ShowToolTip(item.data as ItemDataEquipment);
        ItemDataEquipment oldEquipment = Inventory.instance.GetItemDataEquipment(item.data);
        Inventory.instance.ChangeStatsUI(oldEquipment, item.data as ItemDataEquipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0) return;
        ui.itemTooltip.HideToolTip();
        Inventory.instance.UpdateStatsUI();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0) return;
        originalParent = transform.parent;
        transform.SetParent(transform.parent.parent);
        transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0) return;

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            if (eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotID>() != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotID>().name == "ItemSlot(Clone)")
                {
                    if ( eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>()!= null)
                    {
                        transform.SetParent(eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotID>().transform);
                        transform.position = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().transform.position;
                        SlotID = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotID>().ID;
                        eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().transform.SetParent(originalParent);
                        eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().transform.position = originalParent.position;
                        //Debug.Log(originalParent.GetComponentInParent<SlotID>().ID);
                        eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().SlotID = originalParent.GetComponentInParent<SlotID>().ID;
                        if (eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().item != null)
                        {
                            if (item != null)
                            {
                                eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().item.slotID
                                    = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().SlotID;
                                item.slotID = SlotID;
                            }
                            else
                            {
                                eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().item.slotID
                                    = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIItemSlot>().SlotID;
                            }


                        }
                        else
                        {
                            item.slotID = SlotID;
                        }
                    }
                    else
                    {
                        transform.SetParent(originalParent);
                        transform.position = originalParent.position;
                    }
                }
                else
                {
                    transform.SetParent(originalParent);
                    transform.position = originalParent.position;
                    ItemDataEquipment itemData = item.data as ItemDataEquipment;
                    if (item.data.itemType == ItemType.Equipment &&
                        eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<UIEquipmentSlot>().equipmentType == itemData.equipmentType)
                    {
                        Inventory.instance.EquipItem(item.data);
                    }
                    ui.itemTooltip.HideToolTip();
                   
                }



            }
            else
            {
                transform.SetParent(originalParent);
                transform.position = originalParent.position;
            }
        }
        else
        {

            transform.SetParent(originalParent);
            transform.position = originalParent.position;
            PlayerManager.instance.player.itemDrop.DropItems(item.data, PlayerManager.instance.player.facingDirection);
            Inventory.instance.RemoveItem(item.data);

        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (item == null || item.stackSize == 0) return;
        transform.position = eventData.position;
        //Debug.Log(eventData.pointerCurrentRaycast.gameObject);

    }
}
