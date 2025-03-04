using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrikeContorllers : MonoBehaviour
{
    protected PlayerStats playerStats;
   
    // Start is called before the first frame update
    void Start()
    {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        
    }

  
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()!= null)
        {
           EnemyStats enemyState = collision.GetComponent<EnemyStats>();

            playerStats.DoMagicDamage(enemyState);
        }
    }
    
}
