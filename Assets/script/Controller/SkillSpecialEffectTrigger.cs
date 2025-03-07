using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSpecialEffectTrigger : MonoBehaviour
{
    player1 player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void DestroyTrigger()
    {

        PoolMgr.Instance.Release(gameObject);
    }
    private void ExplodeTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead
                && hit.GetComponent<Enemy>().attackLayerName == "Player")
            {
                player.stats.DoMagicDamage(hit.GetComponent<Enemy>().stats, 1.2f);

                AudioManager.instance.PlaySFX(36, null);
                AttackSense.instance.HitPause(4);
            }
        }
    }
    private void ExplodeTrigger2()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead
                && hit.GetComponent<Enemy>().attackLayerName == "Player")
            {
                player.stats.DoDamage(hit.GetComponent<Enemy>().stats, 2f);

                AudioManager.instance.PlaySFX(36, null);
                AttackSense.instance.HitPause(8);
               
            }
        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null && !collision.GetComponent<CharacterStats>().isDead
            && collision.GetComponent<Enemy>().attackLayerName == "Player")
        {
            player.stats.DoDamage(collision.GetComponent<Enemy>().stats, 4f);
            AudioManager.instance.PlaySFX(36, null);
            AttackSense.instance.HitPause(15);
            // PoolMgr.Instance.Release(gameObject);
        }
    }
}

