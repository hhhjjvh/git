using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : MonoBehaviour
{
    private PolygonCollider2D collider2d;
    private player1 player;
    // Start is called before the first frame update
    void Start()
    {
        collider2d = GetComponent<PolygonCollider2D>();
        player = GetComponentInParent<player1>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer(player.GetAttackLayerName()))
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead)
            {
                
                AudioManager.instance.PlaySFX(1, player.transform);

                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy == null) return;
                player.stats.DoDamage(enemy);

            }
          
        }
        if (hit.GetComponent<ArrowController>() != null)
        {
            AudioManager.instance.PlaySFX(2, player.transform);
            hit.GetComponent<ArrowController>().FlipArrow();
        }
    }
}
