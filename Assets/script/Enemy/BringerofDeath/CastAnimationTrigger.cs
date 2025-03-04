using Unity.Burst.CompilerServices;
using UnityEngine;
public class CastAnimationTrigger : MonoBehaviour
{
    private CharacterStats characterState;
    //private CircleCollider2D cd => GetComponent<CircleCollider2D>();
    private CapsuleCollider2D cd => GetComponent<CapsuleCollider2D>();
    //GameObject parent => transform.parent.gameObject;

    private player1 player => PlayerManager.instance.player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Setup(CharacterStats characterState)
    {
        this.characterState = characterState;

    }
    private void ExplodeTrigger()
    {
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        Collider2D[] colliders = Physics2D.OverlapCapsuleAll(transform.position, cd.size, cd.direction, cd.bounciness);
        foreach (var hit in colliders)
        {
            /*
            if (hit.GetComponent<player1>() != null && !hit.GetComponent<player1>().isDead)
            {
                
                characterState.DoMagicDamage(hit.GetComponent<CharacterStats>(), characterState.GetComponent<Enemy>());
            }
            */
            Enemy enemy = characterState.GetComponent<Enemy>();
            if (hit.GetComponent<entity>() != null)
            {
                // Debug.Log(enemy.GetAttackLayer());
                //Debug.Log(hit.GetComponent<entity>().GetMyLayer());
                if (hit.gameObject.layer == LayerMask.NameToLayer(enemy.GetAttackLayerName()) && !hit.GetComponent<CharacterStats>().isDead)
                {
                    enemy.stats.DoMagicDamage(hit.GetComponent<entity>().stats, enemy);
                }
            }

        }
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<player1>() != null)
        {
           // characterState.DoMagicDamage(collision.GetComponent<CharacterStats>(), characterState.GetComponent<Enemy>());
        }
    }
    private void DestroyTrigger()
    {
        // Destroy(gameObject);
        // Destroy(parent);
        // PoolManager.instance.ReturnToControllerPool(gameObject);
        PoolMgr.Instance.Release(gameObject);
    }
}
