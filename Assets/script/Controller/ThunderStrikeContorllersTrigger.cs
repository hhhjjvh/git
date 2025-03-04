using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrikeContorllersTrigger : MonoBehaviour
{
    private CircleCollider2D cd => GetComponentInParent<CircleCollider2D>();
    float time = 0;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnEnable()
    {
        time = 5;
    }
    // Update is called once per frame
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
    private void ExplodeTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead)
            {
                //hit.GetComponent<Enemy>().TakeDamage(PlayerManager.instance.player.damage);
                // player.stats.DoDamage(hit.GetComponent<CharacterState>());
                AudioManager.instance.PlaySFX(36, null);
               PlayerManager.instance.player.stats.DoMagicDamage(hit.GetComponent<CharacterStats>());
                PoolMgr.Instance.Release(gameObject);
            }
        }
    }
    private void DestroyTrigger()
    {
        // PoolManager.instance.ReturnToControllerPool(gameObject.transform.parent.gameObject);
        PoolMgr.Instance.Release(gameObject.transform.parent.gameObject);
    }
    private void DestroyTrigger2()
    {
        // PoolManager.instance.ReturnToControllerPool(gameObject.transform.parent.gameObject);
        PoolMgr.Instance.Release(gameObject);
    }
}
