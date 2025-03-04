using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireEffect", menuName = "Data/Item effect/FireEffect")]
public class FireEffect : ItemEffect
{
    public override void ExecuteEffect(Transform enemyPosition)
    {
        AudioManager.instance.PlaySFX(32, null);
        AudioManager.instance.PlaySFX(30, null);
        GameObject thunderStrike = PoolMgr.Instance.GetObj("Burn", enemyPosition.position, Quaternion.identity);
    }
}