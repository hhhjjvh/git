using System.Collections;
using UnityEngine;

/*
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(ItemDrop))]
*/

public enum EnemyName
{
    BringerofDeath,
    Skeleton,
    Skeleton_1,
    Skeleton_2,
    Skeleton_3,
    Skeleton_4,
    Skeleton_5,
    Slime_big,
    Slime_small,
    Slime_medium,

}

public class Enemy : entity
{
    public EnemyName enemyName;
    public Vector2 attackKnockback;
    private player1 player;
    // 新增事件：敌人死亡时触发，传递敌人对象和敌人名称
  // public static event System.Action<Enemy, EnemyName> OnEnemyDied;
   
    //public LayerMask attackLayer=> LayerMask.GetMask(attackLayerName);

    // protected bool canBeAtunned;
    [SerializeField] protected GameObject counterImage;
    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    private float defaultMoveSpeed;
    public float ChackDistance=8f;

   public GameObject playerChackTransform;
    public float StartPosition1;
    public float StartPosition2;

    

    private float defaultJumpForce;
    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    public float battleTime;
    public float chaseDistance;
    [HideInInspector] public float lastTimeAttacked { get; set; }

    public EnemyStateMachine stateMachine { get; private set; }
    public string lastAnimBoolName { get; private set; }

    public virtual EnemyIdleState idleState { get; protected set; }
    public virtual EnemyMoveState moveState { get; protected set; }
    public virtual EnemyBattleState battleState { get; protected set; }
    public virtual EnemyAttackBase attackState { get; protected set; }
    public virtual EnemyBigHitState bigHitState { get; protected set; }
    public virtual EnemyHitState hitState { get; protected set; }
    public virtual EnemyDeadState deadState { get; protected set; }
    protected override void Awake()
    {
        base.Awake();

        attackLayerName = "Player";
        stateMachine = new EnemyStateMachine();

        //attackLayer = LayerMask.GetMask("Player");


    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // ;
        enemyName = EnemyName.Skeleton;
        stats = GetComponent<EnemyStats>();
        entityType = entityType.Enemy;
        player = PlayerManager.instance.player;
        defaultMoveSpeed = moveSpeed;

        var ChackPos = new GameObject(name);
        ChackPos.transform.SetParent(transform);

        ChackPos.transform.position = new Vector2(transform.position.x + facingDirection * 1.5f, transform.position.y);
        playerChackTransform = ChackPos;
        
        StartPosition1= transform.position.x - chaseDistance;
        StartPosition2= transform.position.x + chaseDistance;
        
    }
    void OnEnable()
    {
        GetComponent<EnemyStats>().MakeisInvincible(.8f);
        if (anim == null || stateMachine == null || idleState == null)  return;
       
            anim.speed = 1;
            stateMachine.Initialize(idleState);
    }
    void OnDisable()
    {
       if (anim == null || stateMachine == null || idleState == null)  return;
            anim.speed = 1;
            stateMachine.Initialize(idleState);
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Time.timeScale == 0 || !canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        //Debug.Log("Enemy Update");
        base.Update();


        stateMachine.currentState.Update();

        if (ComboCounter != attackState.comboCounter)
        {
            ComboCounter = attackState.comboCounter;
        }
        
    }
   
    public void SetAttackLayerName(string layerName)
    {
        attackLayerName = layerName;
        //attackLayer = LayerMask.GetMask(layerName);
    }
    
    //给自身层级
   



    public virtual void AssigLastAnimName(string animBoolName)
    {
        lastAnimBoolName = animBoolName;
    }
    public virtual void FreezeTimer(bool timeFreeze)
    {
        if (timeFreeze)
        {
            canMove = false;
            anim.speed = 0;

        }
        else
        {
            canMove = true;
            anim.speed = 1;

        }
    }
    public virtual void FreezeTimer(float time) => StartCoroutine(FreezeTimerFor(time));
    protected virtual IEnumerator FreezeTimerFor(float time)
    {
        FreezeTimer(true);
        yield return new WaitForSeconds(time);
        FreezeTimer(false);
    }
    
    public virtual void AniantionTrigger() => stateMachine.currentState.AnimationTrigger();
    public virtual RaycastHit2D CheckPlayerInFront() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, 15, LayerMask.GetMask(attackLayerName));
    //圆型检测
    public virtual Collider2D CheckPlayerInCircle() => Physics2D.OverlapCircle(wallCheck.position, attackDistance * 2, LayerMask.GetMask(attackLayerName));

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDirection, transform.position.y, 0));
        Gizmos.DrawWireSphere(wallCheck.position, attackDistance * 2);
    }

    public virtual void MakeDamageFX(int damage)
    {
       // Debug.Log(GetComponent<CharacterStats>().isOverlordBody);
        if (GetComponent<CharacterStats>().isOverlordBody) return;
        entityFX.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");
    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        float offset = Random.Range(0.7f, 1.3f);
        if (player.primaryAttackState.isPriimaryAttack)
        {
            rb.velocity = new Vector2((player.attackKnockback[player.ComboCounter].x * offset) * player.facingDirection,
            player.attackKnockback[player.ComboCounter].y * offset);
        }
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;

    }
    public void MakeKnockbake(Vector2 knockback, float knockbackDuration)
    {
        //entityFX.StartCoroutine("FlashFX");
        StartCoroutine(HitKnockbackEnemy(knockback, knockbackDuration));

    }
    public IEnumerator HitKnockbackEnemy(Vector2 knockback, float knockbackDuration)
    {
        // Debug.Log("knockback");
        isKnocked = true;
        float offset = Random.Range(0.9f, 1.1f);

        rb.velocity = new Vector2((knockback.x * offset) * player.facingDirection, knockback.y * offset);

        yield return new WaitForSeconds(knockbackDuration);
        //Debug.Log("knockback end");
        isKnocked = false;

    }
    public override void SlowEnityBy(float slowPercent, float slowDuration)
    {

        moveSpeed = moveSpeed * (1 - slowPercent);

        anim.speed = anim.speed * (1 - slowPercent);

        Invoke("ReturnDefaultSpeed", slowDuration);
    }
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;

        anim.speed = 1f;
    }
    public void DestroyMySelf()
    {
        PoolMgr.Instance.Release(gameObject, 10f);
       // Destroy(gameObject, 10);
    }
    public override void Die()
    {
        base.Die();

        // 通过单例队列管理类处理事件

        //  EnemyEventManager.Instance.EnqueueDeathEvent(this, enemyName);
        EnemyEventManager.Instance.EnqueueDeathEvent(this,enemyName); 
        // DestroyMySelf();
    }
   
   
}
