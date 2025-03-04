using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaEffect", menuName = "Data/Item effect/ManaEffect")]

public class ManaEffect : ItemEffect
{
    [Range(0f, 1f)]
    [SerializeField] private float ManaPercent;

    public override void ExecuteEffect(Transform enemyPosition)
    {
        //base.ExecuteEffect(enemyPosition);
        AudioManager.instance.PlaySFX(24, null);
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int Amount = Mathf.RoundToInt(playerStats.GetMaxMana() * ManaPercent);

        playerStats.addMana(Amount);
    }
}
