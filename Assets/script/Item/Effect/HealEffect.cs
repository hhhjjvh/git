using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Data/Item effect/HealEffect")]
public class HealEffect : ItemEffect
{
    [Range(0f,1f)]
    [SerializeField] private float healPercent;

    public override void ExecuteEffect(Transform enemyPosition)
    {
        //base.ExecuteEffect(enemyPosition);
        AudioManager.instance.PlaySFX(24, null);
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealth() * healPercent);

        playerStats.IncreaseHealthBy(healAmount);
    }
}
