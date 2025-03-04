using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UIShopWindow : UICraftWindow
{
    [SerializeField] private Transform StatSlotParent;

    [SerializeField] protected TextMeshProUGUI needMoneytext;
    public TextMeshProUGUI haveMoneytext;

    private int needMoney;
    private UIStatSlot[] statSlots;
    private void Awake()
    {

        statSlots = StatSlotParent.GetComponentsInChildren<UIStatSlot>();
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public  void SetupShopWindow(ItemDataEquipment itemData)
    {
        craftButton.onClick.RemoveAllListeners();
        string combinedText =

       $"<size=40><b>{itemData.itemName}</b></size>\n" +
       $"<size=30><i>{itemData.GetEquipmentTypeName()}</i></size>\n" +  // 斜体物品类型
       $"<size=20><color=#AAAAAA>{itemData.GetDescription()}</color></size>\n" +  // 灰色物品描述
       $"<size=16>{itemData.GetItemName()}</size>\n" +
       $"<size=20><u>{itemData.GetEffectName()}</u></size>";  // 下划线物品效果


        itemName.text = combinedText;// itemData.itemName;
        //itemName.text = itemData.itemName;
        itemDescription.text = itemData.GetDescription();
        imageicon.sprite = itemData.icon;
        needMoney = itemData.needMoney;
        needMoneytext.text ="灵魂："+ needMoney.ToString();
        itemcounttext.text = "持有数量：" + Inventory.instance.GetInventoryCount(itemData).ToString();
        haveMoneytext.text = "拥有灵魂：" + PlayerManager.instance.currency.ToString();
        craftButton.onClick.AddListener(() => Inventory.instance.BuyItem(itemData,needMoney));
        opButton.onClick.AddListener(() => gameObject.SetActive(false));
        opButton.onClick.AddListener(() => UpdateStatsUI());
        SetWindowPosition();
        gameObject.SetActive(true);
        
        ItemDataEquipment oldEquipment = Inventory.instance.GetItemDataEquipment(itemData);
        ChangeStatsUI(oldEquipment, itemData);
    }
    protected override void SetWindowPosition()
    {
        Vector2 mosePosition = Input.mousePosition;
        float xOffset = 0;
        float yOffset = 0;
        if (mosePosition.x > Screen.width / 2)
        {
            xOffset = -200;
        }
        else
        {
            xOffset = 200;
        }
        if (mosePosition.y > Screen.height / 2)
        {
            yOffset = -50;
        }
        else
        {
            yOffset = 200;
        }
        transform.position = new Vector3(mosePosition.x + xOffset, mosePosition.y + yOffset, 0);
    }
    public void UpdateStatsUI()
    {
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

}
