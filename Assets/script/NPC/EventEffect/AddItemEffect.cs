using UnityEngine;



[CreateAssetMenu(menuName = "Event/Effects/Add Item")]
public class AddItemEffect : EventEffect
{
    public ItemData itemData;
    public int count = 1;
    public int needCoin;
    public override void ApplyEffect(GameObject target = null)
    {

        for (int i = 0; i < count; i++)
        {
            Inventory.instance.AddItem(itemData);
        }

    }
    public override bool ApplyTrigger()
    {
        Debug.Log("ApplyTrigger");
        return PlayerManager.instance.HaveEnoughCoin(needCoin);
    }
    public override string GetEffectDescription()
    {
        return $"{itemData.itemName}*{count}";
    }
}