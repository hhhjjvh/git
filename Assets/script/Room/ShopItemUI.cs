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
        // �ϲ����е��ı�����
        string combinedText =
     $"<size=1.2><b>{"��ң�"+count}</b></size>\n" +
     $"<size=0.75><b>{itemData.itemName}</b></size>\n" +
     $"<size=0.6><i>{itemData.GetEquipmentTypeName()}</i></size>\n" +  // б����Ʒ����
     $"<size=0.5><color=#AAAAAA>{itemData.GetDescription()}</color></size>\n" +  // ��ɫ��Ʒ����
     $"<size=0.4>{itemData.GetItemName()}</size>\n" +
     $"<size=0.4><u>{itemData.GetEffectName()}</u></size>";  // �»�����ƷЧ��
        itemNameText.text = combinedText;

        //itemNameText.text = itemData.itemName;

        //itemTypeText.text = itemData.GetEquipmentTypeName();
        //itemDescriptionText.text = itemData.GetDescription();
        //itemText.text = itemData.GetItemName();
        //itemeffectText.text = itemData.GetEffectName();



    }

}
