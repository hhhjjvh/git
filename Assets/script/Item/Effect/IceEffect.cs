using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "IceEffect", menuName = "Data/Item effect/IceEffect")]
public class IceEffect : ItemEffect
{
    public override void ExecuteEffect(Transform enemyPosition)
    {
        AudioManager.instance.PlaySFX(31, null);

        GameObject thunderStrike = PoolMgr.Instance.GetObj("Burst of ice", enemyPosition.position, Quaternion.identity);
    }
}
