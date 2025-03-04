using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThunderStrikeEffect", menuName = "Data/Item effect/ThunderStrikeEffect")]
public class ThunderStrikeEffect :ItemEffect
{
    //[SerializeField] private GameObject thunderStrikePrefab;
    public override void ExecuteEffect(Transform enemyPosition)
    {
        AudioManager.instance.PlaySFX(14, null);
        // base.ExecuteEffect();
        // GameObject thunderStrike = Instantiate(thunderStrikePrefab, enemyPosition.position, Quaternion.identity);
        // Destroy(thunderStrike, 1f);
        //GameObject thunderStrike = PoolManager.instance.GetControllerFromPool("Thunderstrike");
        //thunderStrike.transform.position = enemyPosition.position;
        GameObject thunderStrike = PoolMgr.Instance.GetObj("Thunderstrike", enemyPosition.position, Quaternion.identity);
    }
}
