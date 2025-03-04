
using UnityEngine;


public class BrownTreasureBox : ITreasureBox
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
        // WeaponFactory.Instance.GetPlayerWeaponObj(GetWeaponType(), transform.Find("WeaponCreatePoint").position);
    }
    /*
    private PlayerWeaponType GetWeaponType()
    {
        if (ModelContainer.Instance.GetModel<SceneModel>().sceneName == SceneName.MiddleScene)
        {
            if (!ModelContainer.Instance.GetModel<MemoryModel>().isOnlineMode)
            {
                switch (Random.Range(0, 5))
                {
                    case 0:
                        return PlayerWeaponType.DesertEagle;
                    case 1:
                        return PlayerWeaponType.SnowFoxL;
                    case 2:
                        return PlayerWeaponType.AK47;
                    case 3:
                        return PlayerWeaponType.AssaultRifle;
                    case 4:
                        return PlayerWeaponType.UZI;
                }
            }
            else
            {
                return PlayerWeaponType.PKP;
            }
        }
        else
        {
            return PlayerWeaponType.PKP;
        }

        return PlayerWeaponType.PKP;
    }
    */
}
