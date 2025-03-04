using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemText : MonoBehaviour
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;
    float time;
  
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
      
    }
    public void SetupItemText(ItemData itemData)
    {
      itemImage.sprite = itemData.icon;
      itemText.text = itemData.itemName;
        time = 3;
    }
}
