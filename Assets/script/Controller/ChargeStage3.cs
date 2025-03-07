using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeStage3 : ThunderStrikeContorllers
{
    float time = 0;
    public float damage = 2.5f;
    void OnEnable()
    {
        time = 1;
    }
    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            PoolMgr.Instance.Release(gameObject);
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null && !collision.GetComponent<CharacterStats>().isDead)
        {
            EnemyStats enemyState = collision.GetComponent<EnemyStats>();

            playerStats.DoDamage(enemyState,damage);
            AttackSense.instance.HitPause((int)(3*damage));
            AudioManager.instance.PlaySFX(36, null);
        }
    }
}
