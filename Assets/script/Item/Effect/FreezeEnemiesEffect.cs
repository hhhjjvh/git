using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "FreezeEnemiesEffect", menuName = "Data/Item effect/FreezeEnemiesEffect")]
public class FreezeEnemiesEffect : ItemEffect
{
    [SerializeField] private float duration;

    public override void ExecuteEffect(Transform enemyPosition)
    {
       
        // Debug.Log("FreezeEnemiesEffect");
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        if(playerStats.health>playerStats.GetMaxHealth()*0.5) return;

        if(!Inventory.instance.CanUseArmor())  return;
        AudioManager.instance.PlaySFX(27, null);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyPosition.position, 2);
        PlayerManager.instance.player.entityFX.CreatePopUpText(" ±º‰Õ£÷π", Color.green);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead)
            {
                hit.GetComponent<Enemy>().FreezeTimer(duration);
            }
          
        }
    }
}
