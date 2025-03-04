using TMPro;
using UnityEngine;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI itemeffectText;

    // private int fontSize = 32;


    public void ShowToolTip(ItemDataEquipment itemData,int count)

    {
        if (itemData == null) return;
        // 合并所有的文本内容
        string combinedText =
     $"<size=1.2><b>{"金币："+count}</b></size>\n" +
     $"<size=0.75><b>{itemData.itemName}</b></size>\n" +
     $"<size=0.6><i>{itemData.GetEquipmentTypeName()}</i></size>\n" +  // 斜体物品类型
     $"<size=0.5><color=#AAAAAA>{itemData.GetDescription()}</color></size>\n" +  // 灰色物品描述
     $"<size=0.4>{itemData.GetItemName()}</size>\n" +
     $"<size=0.4><u>{itemData.GetEffectName()}</u></size>";  // 下划线物品效果
        itemNameText.text = combinedText;

        //itemNameText.text = itemData.itemName;

        //itemTypeText.text = itemData.GetEquipmentTypeName();
        //itemDescriptionText.text = itemData.GetDescription();
        //itemText.text = itemData.GetItemName();
        //itemeffectText.text = itemData.GetEffectName();



    }

}
