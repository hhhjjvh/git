using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CloneSkillController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
   // private player1 player;
    
    
    [SerializeField] private float colorLoosingSpeed;
    private float cloneduurationTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius=.8f;
    private player1 player;
    private Transform closestEnemy;
    private bool canDuplicateClone;
    private int facingDirection=1;
    private float chanceToDuplicate;
    private float attackMultiplier;

    private bool Duplicated;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
       
        //player = GameObject.FindGameObjectWithTag("Player");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<player1>();


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float alpha = spriteRenderer.color.a - colorLoosingSpeed * Time.deltaTime;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        if (spriteRenderer.color.a <= 0)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            //Destroy(gameObject);
            // PoolManager.instance.ReturnToControllerPool(gameObject);
            PoolMgr.Instance.Release(gameObject);
        }
        animator.speed = player.anim.speed*2;
        /*
        cloneduurationTimer -= Time.deltaTime;
        if (cloneduurationTimer <= 0)
        {
            spriteRenderer.color=new Color(1, 1, 1, spriteRenderer.color.a - (colorLoosingSpeed*Time.deltaTime));
        }
        if (spriteRenderer.color.a <= 0)
        {
            Destroy(gameObject);
        }
        */
    }
    public void SetupClone(Transform transform,float cloneduuration,bool canAttack,Vector3 offset,Transform closestEnemy,bool canDuplicateClone
        ,float chanceToDuplicate,float attackMultiplier,bool canDuplicate)
    {
        if (canAttack)
        {
            animator.SetInteger("AttackNumber", Random .Range(1, 4));
        }
        this.transform.position = transform.position+offset;
        cloneduurationTimer = cloneduuration;
        //spriteRenderer.color = new Color(1, 1, 1, 1);
        this.closestEnemy = closestEnemy;
        this.canDuplicateClone = canDuplicateClone;
        this.chanceToDuplicate = chanceToDuplicate;
        this.attackMultiplier = attackMultiplier;
        this.Duplicated = canDuplicate;
      
        FaceClosestTarget();
    }
    public void AnimationEnterTrigger()
    {
        AudioManager.instance.PlaySFX(0, null);
    }
    private void AnimationTrigger()
    {
       cloneduurationTimer=0.1f;
        animator.SetInteger("AttackNumber", 5);
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position,attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null&&!hit.GetComponent<CharacterStats>().isDead)
            {
               // AudioManager.instance.PlaySFX(1, player.transform);
                // hit.GetComponent<Enemy>().TakeDamage(player.damage);
                //player.stats.DoDamage(hit.GetComponent<CharacterStats>());
                player.entityFX.ScreenShake(0.2f, 0.2f);
                PlayerStats playerStats = player.GetComponent<PlayerStats>();   
                EnemyStats enemyStats = hit.GetComponent<EnemyStats>();
                playerStats.CloneDoDamage(enemyStats, attackMultiplier);
                //Debug.Log("Clone Attack");
                if(player.skillManager.clone.canAggresiveClone)
                {
                    ItemDataEquipment weapon = Inventory.instance.GetEquipmentType(EquipmentType.Weapon);

                    if (weapon != null)
                    {
                        weapon.ItemEffect(hit.transform);
                    }
                }


                if (canDuplicateClone&&!Duplicated)
                {
                    int chance = Random.Range(0, 100);
                   
                    if (chance < chanceToDuplicate)
                    {
                     // Debug.Log("Clone Duplicate");
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(2f*facingDirection, 0),true);
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(-2f * facingDirection, 0), true);

                    }
                }
                return;
            }
        }
    }
    private void OpenAttackTrigger()
    {
       // player.OpenCounterAttackWindow();

    }
    private void CloseAttackTrigger()
    {
      //  player.CloseCounterAttackWindow();
    }
    private void FaceClosestTarget()
    {
       
        if (closestEnemy != null)
        {
            if (closestEnemy.position.x < transform.position.x)
            {
                facingDirection = -1;
                transform.Rotate(0, 180, 0);
            }
        }
         
    }

}
