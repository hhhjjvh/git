using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private TextMeshProUGUI itemeffectText;

    private int fontSize = 32;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowToolTip(ItemDataEquipment itemData)

    {
        if(itemData == null) return;
        string combinedText =

    $"<size=40><b>{itemData.itemName}</b></size>\n" +
    $"<size=30><i>{itemData.GetEquipmentTypeName()}</i></size>\n" +  // б����Ʒ����
    $"<size=20><color=#AAAAAA>{itemData.GetDescription()}</color></size>\n" +  // ��ɫ��Ʒ����
    $"<size=16>{itemData.GetItemName()}</size>\n" +
    $"<size=20><u>{itemData.GetEffectName()}</u></size>";  // �»�����ƷЧ��


        //  itemName.text = combinedText;// itemData.itemName;
        itemNameText.text = combinedText; //itemData.itemName;

        itemTypeText.text = itemData.GetEquipmentTypeName();
        itemDescriptionText.text = itemData.GetDescription();
        itemText.text = itemData.GetItemName();
        itemeffectText.text = itemData.GetEffectName();


        if(itemNameText.text.Length>12)
        {
            itemNameText.fontSize *= 0.7f;
        }
        else
        {
            itemNameText.fontSize = fontSize;
        }


        gameObject.SetActive(true);
        Vector2 mosePosition = Input.mousePosition;
        float xOffset = 0;
        float yOffset = 0;
        if (mosePosition.x > Screen.width / 2)
        {
            xOffset = 0;
        }
        else
        {
            xOffset = 200;
        }
        if (mosePosition.y > Screen.height / 2)
        {
            yOffset = -200;
        }
        else
        {
            yOffset = 200;
        }
       transform.position = new Vector3(mosePosition.x + xOffset, mosePosition.y + yOffset, 0);

    }

    public void HideToolTip()
    { 
        itemNameText.fontSize = fontSize;
        gameObject.SetActive(false);
    }

}
