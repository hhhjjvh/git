using UnityEngine;

public class GoldTreasureBox : ITreasureBox
{
    protected override void OnFinishOpen()
    {
        base.OnFinishOpen();
        if (dropItems.Count > 0)
        {
            GameObject weapon = PoolMgr.Instance.GetObj("Item _Sword", transform.position);
            ItemData dropItem = dropItems[Random.Range(0, dropItems.Count - 1)];
            Vector2 randomDirection = new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f));
            weapon.GetComponent<ItemObject>().SetUpItem(dropItem, randomDirection);
            weapon.transform.SetParent(transform.parent);
        }
    }
}
