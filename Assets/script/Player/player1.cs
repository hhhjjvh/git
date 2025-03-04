using System.Collections;
using UnityEngine;

public class player1 : entity
{
    public PlayerItemDrop itemDrop;
    [SerializeField] public Vector2 knockback;

    [Header("Attack info")]
    public float[] attackMovement;
    public Vector2[] attackKnockback;
    public float counterattackDuration;

    public bool isBusy { get; private set; }
    [Header("Move info")]
    public float moveSpeed;
    public float climbSpeed;
    public float flySpeed;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;
    [Header("Dash info")]
    // [SerializeField] private float dashCooldown;
    //private float dashTime;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    private float defaultDashSpeed;
    public float dashDir { get; set; }

    public bool isJumped;

    public float slideSpeed = 15f;
    public float slideDuration = 1f;

    
   public AttackCheck attackChecked;

    [SerializeField] protected Transform airCheck;
    [SerializeField] protected Transform wallCheck2;
    [SerializeField] protected Transform wallCheck3;
    [SerializeField] protected Vector2 wallCheck3Size;
    [SerializeField] protected float airCheckDistance;
    [SerializeField] protected Transform LadderChack;
    [SerializeField] protected Transform LadderChack2;
    [SerializeField] protected Transform LadderChack3;
    [SerializeField] protected Vector2 LadderChackSize;
    [SerializeField] protected Vector2 LadderChackSize2;
    //[SerializeField] protected LayerMask ladderLayer;


    public SkillManager skillManager { get; private set; }
    public GameObject sword { get; private set; }


    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState fallState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerDashAttack dashAttackState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPriimaryAttackState primaryAttackState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCathSwordState cathSwordState { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerClimbState climbState { get; private set; }
    public PlayerWallGrabState wallGrabState { get; private set; }
    public PlayerSlideState slideState { get; private set; }
    public PlayerParryState parryState { get; private set; }
    public PlayerFlyState flyState { get; private set; }
    public PlayerHitState hitState { get; private set; }
    public PlayerCroushState croushState { get; private set; }
    public PlayerUseSkillState useSkillState { get; private set; }
    public PlayerUseSkillWithBigState useSkillWithBigState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        attackLayerName = "Enemy";
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(stateMachine, this, "Idle");
        moveState = new PlayerMoveState(stateMachine, this, "Move");
        runState = new PlayerRunState(stateMachine, this, "Run");
        jumpState = new PlayerJumpState(stateMachine, this, "Jump");
        fallState = new PlayerAirState(stateMachine, this, "Jump");
        dashState = new PlayerDashState(stateMachine, this, "Dash");
        dashAttackState = new PlayerDashAttack(stateMachine, this, "DashAttack");
        wallSlideState = new PlayerWallSlideState(stateMachine, this, "WallSlide");
        wallJumpState = new PlayerWallJumpState(stateMachine, this, "Jump");
        primaryAttackState = new PlayerPriimaryAttackState(stateMachine, this, "Attack");
        counterAttackState = new PlayerCounterAttackState(stateMachine, this, "ContterAttack");
        aimSwordState = new PlayerAimSwordState(stateMachine, this, "AimSword");
        cathSwordState = new PlayerCathSwordState(stateMachine, this, "CathSword");
        blackholeState = new PlayerBlackholeState(stateMachine, this, "Jump");
        deadState = new PlayerDeadState(stateMachine, this, "Dead");
        climbState = new PlayerClimbState(stateMachine, this, "Climb");
        wallGrabState = new PlayerWallGrabState(stateMachine, this, "WallGrab");
        slideState = new PlayerSlideState(stateMachine, this, "Slide");
        parryState = new PlayerParryState(stateMachine, this, "Parry");
        flyState = new PlayerFlyState(stateMachine, this, "Fly");
        hitState = new PlayerHitState(stateMachine, this, "Hit");
        croushState = new PlayerCroushState(stateMachine, this, "Croush");
        useSkillState = new PlayerUseSkillState(stateMachine, this, "UseSkill");
        useSkillWithBigState = new PlayerUseSkillWithBigState(stateMachine, this, "Jump");
        stats = GetComponent<PlayerStats>();

    }
    protected override void Start()
    {
        base.Start();
        entityType = entityType.Player;

        itemDrop = GetComponent<PlayerItemDrop>();

        stats = GetComponent<PlayerStats>();

        //attackChecked = GetComponentInChildren<AttackCheck>();

        skillManager = SkillManager.instance;
        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }
    protected override void Update()
    {
        if (Time.timeScale == 0 || !canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        base.Update();
        stateMachine.currentState.Update();

        // CheckForDashInput();

        ComboCounter = primaryAttackState.GetComboCounter();


        if (InputManager.Instance.canUseItem)
        {
            InputManager.Instance.canUseItem = false;
            Inventory.instance.UseFlask();
        }

      
    }
    public override void OpenCounterAttackWindow()
    {
        canBeAtunned = true;
        attackChecked.gameObject.SetActive(true);
        //counterImage.SetActive(true);
    }
    public override void CloseCounterAttackWindow()
    {
        canBeAtunned = false;
        attackChecked.gameObject.SetActive(false);
        // counterImage.SetActive(false);
    }
    public virtual bool IsairDetected() => Physics2D.Raycast(airCheck.position, Vector2.right, airCheckDistance * facingDirection, wallLayer);
    public virtual bool IsWallDetected2() => Physics2D.Raycast(wallCheck2.position, Vector2.right, wallCheckDistance * facingDirection, wallLayer);

    public virtual bool IsWallDetected3() => Physics2D.BoxCast(wallCheck3.position, wallCheck3Size, 0, Vector2.zero, 0, wallLayer);

    public virtual bool IsLadderDetected() => Physics2D.BoxCast(LadderChack.position, LadderChackSize, 0, Vector2.zero, 0, LayerMask.GetMask("Ladder"));
    public virtual bool IsLadderDetected2() => Physics2D.BoxCast(LadderChack2.position, LadderChackSize, 0, Vector2.zero, 0, LayerMask.GetMask("Ladder"));

    public virtual bool IsLadderDetected3() => Physics2D.BoxCast(LadderChack3.position, LadderChackSize2, 0, Vector2.zero, 0, LayerMask.GetMask("Ladder"));

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(airCheck.position, new Vector3(airCheck.position.x + airCheckDistance * facingDirection, airCheck.position.y));
        Gizmos.DrawLine(wallCheck2.position, new Vector3(wallCheck2.position.x + wallCheckDistance * facingDirection, wallCheck2.position.y));
        Gizmos.DrawWireCube(LadderChack.position, LadderChackSize);
        Gizmos.DrawWireCube(LadderChack2.position, LadderChackSize);
        Gizmos.DrawWireCube(LadderChack3.position, LadderChackSize2);
        Gizmos.DrawWireCube(wallCheck3.position, wallCheck3Size);
    }
    public void AssignNewSword(GameObject sword)
    {
        this.sword = sword;
    }
    public void CatchTheSword()
    {
        stateMachine.ChangeState(cathSwordState);
        Destroy(sword);
    }
    public void ExitBlackHoleAbility()
    {
        stateMachine.ChangeState(fallState);
        // Debug.Log("exit black hole");
    }

    public IEnumerable BusyFor(float duration)
    {
        isBusy = true;
        yield return new WaitForSeconds(duration);
        isBusy = false;
    }
    public void AniantionTrigger() => stateMachine.currentState.AnimationTrigger();
    private void CheckForDashInput()
    {
        if (IsWallDetected() && !IsGroundedDetected()) return;
        if (!skillManager.dash.dashUnlocked) return;
        //dashTime -= Time.deltaTime;
        if (InputManager.Instance.canDash && SkillManager.instance.dash.CanUseSkill())
        {
            InputManager.Instance.canDash = false;
            //dashTime = dashCooldown;
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
            {
                dashDir = facingDirection;
            }

            stateMachine.ChangeState(dashState);
        }
    }

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        rb.velocity = new Vector2(knockback.x * -facingDirection, knockback.y);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;

    }
    public virtual void MakeDamageFX(int damage, Enemy enemy)
    {

        if (!GetComponent<CharacterStats>().isDead && !GetComponent<CharacterStats>().isOverlordBody)
        {
            entityFX.StartCoroutine("FlashFX");
            stateMachine.ChangeState(hitState);
            if (enemy != null)
            {
                StartCoroutine("HitKnockbacks", enemy);
            }
        }
        // Debug.Log("player take damage");
    }
    protected virtual IEnumerator HitKnockbacks(Enemy enemy)
    {
        isKnocked = true;
        rb.velocity = new Vector2(enemy.attackKnockback.x * enemy.facingDirection, enemy.attackKnockback.y);
        yield return new WaitForSeconds(enemy.knockbackDuration);
        isKnocked = false;

    }
    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
    public override void SlowEnityBy(float slowPercent, float slowDuration)
    {
        //base.SlowEnityBy(slowPercent, slowDuration);
        moveSpeed = moveSpeed * (1 - slowPercent);
        jumpForce = jumpForce * (1 - slowPercent);
        dashSpeed = dashSpeed * (1 - slowPercent);
        anim.speed = anim.speed * (1 - slowPercent);
        //Debug.Log("slow down");
        Invoke("ReturnDefaultSpeed", slowDuration);
    }
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
        anim.speed = 1.5f;
    }
}
