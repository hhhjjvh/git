using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CrystalSkillController : MonoBehaviour
{
    private Animator animator=> GetComponent<Animator>();
    private CircleCollider2D cd=> GetComponent<CircleCollider2D>();
    private float CrystalExistTime;
    private player1 player;

    private float moveSpeed;
    private bool canMove;
    private bool canExplode= true;
    private bool canGrow;
    private float growSpeed=5;
    private Transform closeenemy;
    [SerializeField] private LayerMask enemyLayer;
    public void SetCrystal(float time, float speed, bool canMove, bool canExplode, Transform closeenemy)
    {
        CrystalExistTime = time;
        this.moveSpeed = speed;
        this.canMove = canMove;
        this.canExplode = canExplode;
        this.closeenemy = closeenemy;
        // transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        canGrow = false;
        transform.localScale = Vector3.one;
        StartCoroutine(returnpool(5f, gameObject));
    }
    public void ChosseRandomEnemy()
    {
        float radius=SkillManager.instance.blackhole.GetRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
        if (colliders.Length <= 0) return;
        closeenemy=colliders[Random.Range(0, colliders.Length)].transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        CrystalExistTime -= Time.deltaTime;
        if(CrystalExistTime <0)
        {
            FinishCrystal();
        }
        if(canGrow)
        {
            transform.localScale= Vector2.Lerp(transform.localScale,new Vector2(3,3), growSpeed * Time.deltaTime);
        }
        if (canMove)
        {
            if (closeenemy == null) return;
            transform.position = Vector2.MoveTowards(transform.position, closeenemy.position, moveSpeed * Time.deltaTime);
            {
                if (Vector2.Distance(transform.position, closeenemy.position) < 0.1f)
                {
                    canMove = false;
                    FinishCrystal();
                }
            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow=true;
            canExplode= false;
            animator.SetTrigger("Explode");
        }
       
    }
    private void ExplodeTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead)
            {
                //hit.GetComponent<Enemy>().TakeDamage(PlayerManager.instance.player.damage);
                //player.stats.DoDamage(hit.GetComponent<CharacterState>());
                player.stats.DoMagicDamage(hit.GetComponent<CharacterStats>());
                AudioManager.instance.PlaySFX(34, null);
                ItemDataEquipment equipment = Inventory.instance.GetEquipmentType(EquipmentType.Amulet);
                if (equipment != null)
                {
                    equipment.ItemEffect(hit.transform);
                }
            }
        }
    }
    private void DestroyTrigger()
    {
        // Destroy(gameObject);
        // PoolManager.instance.ReturnToControllerPool(gameObject);
        PoolMgr.Instance.Release(gameObject);
    }
    IEnumerator returnpool(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        // PoolManager.instance.ReturnToControllerPool(obj);
        PoolMgr.Instance.Release(obj);

    }
}
