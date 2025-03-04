using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/ItemDatas")]
public class ItemDatas : ScriptableObject
{
    public TextAsset textAsset;
    public List<ItemData> attrs = new List<ItemData>();
    private void OnValidate()
    {
       //UnityTool.Instance.WriteDataToListFromTextAssest(attrs, textAsset);
    }
}
