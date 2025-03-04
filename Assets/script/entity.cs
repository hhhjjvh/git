
using System.Collections.Generic;
using UnityEngine;
public enum entityType
{
    Player,
    Enemy
}
public class entity : MonoBehaviour
{
    public string attackLayerName;
    public bool isAttack { get; set; }
    //public bool isDead {  get; set; }
    public bool isHitOver { get; set; }
    public bool isParry;
    public bool isAnimationStop;
    public int damage;
    public int ComboCounter;

    public bool canBeAtunned;

    public entityType entityType;
    [Header("Knockback info")]
    //[SerializeField] public Vector2 knockback;
    [SerializeField] public float knockbackDuration;
    protected bool isKnocked;
    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    public LayerMask OneWayPlatformLayer;
    [Header("Slope Detection")]
    //[SerializeField] private Transform slopeCheck;
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 85f;
    
    public Vector2 slopeNormalPerp { get; private set; }
   public bool isOnSlope;

    public  int facingDirection{ get; protected set; } = 1;
    
    
    protected  bool facingRight = true;
   public bool canMove= true;
    public virtual void CanMove(bool canMove)
    {
        this.canMove = canMove;
    }
    public virtual void Rest()
    {
        canMove = true;
        isKnocked = false;
        facingDirection = 1;
        facingRight = true;
    }

    public System.Action onFlipped { get;set; }

    public HealthBarUI healthBarUI { get; private set; }

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX entityFX { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public CharacterStats stats { get;protected set; }
    public CapsuleCollider2D cd { get; private set; }
    public int _frameCount { get; private set; } = 0;
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        entityFX = GetComponentInChildren<EntityFX>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
        healthBarUI = GetComponentInChildren<HealthBarUI>();
        slopeCheckDistance = groundCheckDistance * 2f;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
       
        //OneWayPlatformLayer= LayerMask.NameToLayer("OneWayPlatform");
        OneWayPlatformLayer = LayerMask.GetMask("OneWayPlatform");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
      //  _frameCount++;
       // if ((rb.velocity.magnitude > 0.1f || !IsGroundedDetected()))
            SlopeCheck();
    }
    
    private void SlopeCheck()
    {
        
        if (groundCheck == null || cd == null)
        {
            Debug.LogError("Missing critical components!");
            return;
        }

        // 动态计算射线参数
        float colliderWidth = cd.bounds.size.x;
        int rayCount = Mathf.CeilToInt(colliderWidth / 0.2f);
        float startOffset = -colliderWidth / 2;
        float step = colliderWidth / (rayCount - 1);

        List<RaycastHit2D> validHits = new List<RaycastHit2D>();

        // 发射多方向射线
        for (int i = 0; i < rayCount; i++)
        {
            float offsetX = startOffset + i * step;
            Vector2 origin = groundCheck.position + new Vector3(offsetX, 0);

            // 垂直向下射线
            RaycastHit2D hitVertical = Physics2D.Raycast(origin, Vector2.down, slopeCheckDistance, groundLayer);
            if (hitVertical) validHits.Add(hitVertical);

            // 向前下方发射射线（角度可调）
            Vector2 forwardDir = new Vector2(facingDirection * 0.3f, -1).normalized; // 30度倾斜
            RaycastHit2D hitForward = Physics2D.Raycast(origin, forwardDir, slopeCheckDistance * 1.5f, groundLayer);
            if (hitForward) validHits.Add(hitForward);
        }

        // 计算有效法线
        if (validHits.Count > 0)
        {
            Vector2 weightedNormal = Vector2.zero;
            foreach (var hit in validHits)
            {
                float weight = 1 / (hit.distance + 0.01f); // 防止除零
                weightedNormal += hit.normal * weight;
            }
            weightedNormal.Normalize();

            float slopeAngle = Vector2.Angle(weightedNormal, Vector2.up);
            isOnSlope = slopeAngle <= maxSlopeAngle && slopeAngle != 0;
            slopeNormalPerp = Vector2.Perpendicular(weightedNormal).normalized;
        }
        else
        {
            isOnSlope = false;
        }
    }

    public virtual void OpenCounterAttackWindow()
    {
        canBeAtunned = true;
        //counterImage.SetActive(true);
    }
    public virtual void CloseCounterAttackWindow()
    {
        canBeAtunned = false;
        // counterImage.SetActive(false);
    }
    public virtual bool CanBeStunned()
    {
        if (canBeAtunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }
    public virtual string GetAttackLayerName()
    {
        return attackLayerName;
    }
    #region Velocity
    public void SetVelocity(float x, float y)
    {
        if(isKnocked) return;
        rb.velocity = new Vector2(x, y);
        FlipControllers(x);
    }
    public void ZeroVelocity()
    {
        if(isKnocked) return;
        rb.velocity = Vector2.zero;
    }
    #endregion
    #region Collison
    public virtual bool IsGroundedDetected()
    {
      if(Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer)
            || Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, OneWayPlatformLayer))
        {
            return true;
        }

        return false;
    }
   

   
    public virtual bool IsOneWayPlatformDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, OneWayPlatformLayer);

    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance * facingDirection, wallLayer);
    public virtual bool IsBehindWallDetected() =>Physics2D.Raycast(wallCheck.position, Vector2.left, 5 * facingDirection, wallLayer);
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x -5 * facingDirection, wallCheck.position.y));
        //Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * slopeCheckDistance);
        // 绘制多射线检测
        if (groundCheck != null && cd != null)
        {
            float colliderWidth = cd.bounds.size.x;
            int rayCount = Mathf.CeilToInt(colliderWidth / 0.2f);
            float startOffset = -colliderWidth / 2;
            float step = colliderWidth / (rayCount - 1);

            for (int i = 0; i < rayCount; i++)
            {
                float offsetX = startOffset + i * step;
                Vector3 origin = groundCheck.position + new Vector3(offsetX, 0);

                // 绘制垂直射线
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(origin, origin + Vector3.down * slopeCheckDistance);

                // 绘制倾斜射线
                Vector2 forwardDir = new Vector2(facingDirection * 0.3f, -1).normalized;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin, origin + (Vector3)(forwardDir * slopeCheckDistance * 1.5f));
            }
        }
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion
    #region Flip
    public virtual void Flip()
    {

        facingDirection *= -1;
        
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        if (onFlipped != null)
        {
            //Debug.Log("onFlip");
            onFlipped();
        }

    }
    public virtual void FlipControllers(float x)
    {
        if (facingRight && x < 0 || !facingRight && x > 0)
        {
            Flip();
        }

    }
    #endregion
    public void MakeTransprent(bool isTransparent)
    {
        if (isTransparent)
        {
            spriteRenderer.color = Color.clear;

        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
    public virtual void Die()
    {
        anim.speed *= 0.8f;
    }
    public virtual void SlowEnityBy(float slowPercent,float slowDuration)
    {

    }
    protected virtual void ReturnDefaultSpeed()
    {
        
    }
   
}
