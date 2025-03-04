using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICraftWindow : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI itemName;
    [SerializeField] protected TextMeshProUGUI itemDescription;
    [SerializeField] protected Image imageicon;
    [SerializeField] protected Button craftButton;
    [SerializeField] protected Button opButton;
    public TextMeshProUGUI itemcounttext;

    public Image[] materialImage;
    // Start is called before the first frame update
    void Start()
    {
       // GameObject go = GameObject.Find("Listicon");
       // materialImage= GameObject.Find("Listicon").GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
       // if(craftButton.onClick)
      
        
    }
    public virtual void SetupCraftWindow(ItemDataEquipment itemData)
    {
        craftButton.onClick.RemoveAllListeners();
        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;

        }

        for (int i = 0; i < itemData.craftRequirements.Count; i++)
        {
            if (itemData.craftRequirements.Count > materialImage.Length)
            {


            }

            materialImage[i].sprite = itemData.craftRequirements[i].data.icon;
            materialImage[i].color = Color.white;
            int havestack = Inventory.instance.GetStashCount(itemData.craftRequirements[i].data);
            int needstack = itemData.craftRequirements[i].stackSize;

            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().text = havestack.ToString() + "/" + needstack.ToString();
            if (havestack >= needstack)
            {
                materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().fontSize = 16;

        }
      
    string combinedText =
     
     $"<size=40><b>{itemData.itemName}</b></size>\n" +
     $"<size=30><i>{itemData.GetEquipmentTypeName()}</i></size>\n" +  // 斜体物品类型
     $"<size=20><color=#AAAAAA>{itemData.GetDescription()}</color></size>\n" +  // 灰色物品描述
     $"<size=16>{itemData.GetItemName()}</size>\n" +
     $"<size=20><u>{itemData.GetEffectName()}</u></size>";  // 下划线物品效果


        itemName.text = combinedText;// itemData.itemName;
        itemDescription.text = itemData.GetDescription();
        imageicon.sprite = itemData.icon;
        itemcounttext.text = "持有数量：" + Inventory.instance.GetInventoryCount(itemData).ToString();
        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(itemData, itemData.craftRequirements));
        opButton.onClick.AddListener(() => gameObject.SetActive(false));
        SetWindowPosition();
        gameObject.SetActive(true);
    }

    protected virtual  void SetWindowPosition()
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
            yOffset = -200;
        }
        else
        {
            yOffset = 200;
        }
        transform.position = new Vector3(mosePosition.x + xOffset, mosePosition.y + yOffset, 0);
    }
}
