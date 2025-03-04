using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class SwordSkillController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private player1 player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float speed = 12f;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;

    [Header("Bounce info")]
    private bool isBouncing;
    private float bounceSpeed;
    private int amountOfBounces;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;


    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        
    }
    private void Destroyme()
    {
        Destroy(gameObject);
    }
   
    // Update is called once per frame
    void Update()
    {
       
        if (canRotate)
        {
            transform.right = rb.velocity;
        }
        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
            if (Vector2.Distance(transform.position, player.transform.position) < 0.1f)
            {
                player.CatchTheSword();
            }
        }
        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }
            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;
                //Debug.Log(spinTimer);
                transform.position = Vector2.MoveTowards(transform.position,
                    new Vector2(transform.position.x+ spinDirection, transform.position.y),1.5f*Time.deltaTime);
                if (spinTimer <= 0)
                {
                    isSpinning = false;
                    isReturning = true;
                }
                hitTimer -= Time.deltaTime;
                if (hitTimer <= 0)
                {
                    hitTimer = hitCooldown;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.GetComponent<Enemy>() != null)
                        {
                           // collider.GetComponent<Enemy>().TakeDamage(1);
                           SwordSkillDamage(collider.GetComponent<Enemy>());
                        }
                    }
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        //rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
       
    }

    private void BounceLogic()
    {
       
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, Time.deltaTime * bounceSpeed);
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {

                // enemyTarget[targetIndex].GetComponent<Enemy>().TakeDamage(1);
                //enemyTarget[targetIndex].GetComponent<Enemy>().StartCoroutine("FreezeTimerFor", freezeTimeDuration);
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
                targetIndex++;
                amountOfBounces--;
                if (amountOfBounces <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
                if (targetIndex >= enemyTarget.Count)
                {
                    targetIndex = 0;
                }

            }
        }
    }

    public void SetupSword(Vector2 direction,float gravityScale,player1 player,float freezeTimeDuration,float returnspeed)
    {
        this.player = player;
        this.freezeTimeDuration = freezeTimeDuration;
        this.speed = returnspeed;

        rb.velocity = direction;
        rb.gravityScale = gravityScale;
        if (pierceAmount <= 0)
        {
            animator.SetBool("Rotation", true);
        }
        spinDirection = Mathf.Clamp(rb.velocity.x, -1f, 1f);

        //Invoke("Destroyme", 7f);
        Invoke("ReturnSword", 4f);
    }
    public void SetupBounce(bool isBounce,int amountOfBounces, float bounceSpeed)
    {
        isBouncing = isBounce;
        this.amountOfBounces = amountOfBounces;
        this.bounceSpeed = bounceSpeed;

        enemyTarget = new List<Transform>();
    }
    public void SetupPierce(int pieceAmount)
    {
        pierceAmount = pieceAmount;
    }

    public void SetupSpin(bool isSpin,float spinDuration, float maxTravelDistance,float hitCooldown)
    {
        isSpinning = isSpin;
        this.spinDuration = spinDuration;
        this.maxTravelDistance = maxTravelDistance;
        this.hitCooldown = hitCooldown;

    }
    public void ReturnSword()
    {
        //animator.SetBool("Rotation", true);

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;

        transform.parent = null;
        isReturning = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning) return;

        if (collision.GetComponent<Enemy>() != null&&collision.GetComponent<CharacterStats>().isDead==false)
        {
            // collision.GetComponent<Enemy>()?.TakeDamage(10);
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
            StuckInto(collision);
        }
        if (collision.GetComponent<ArrowController>() != null)
        {
            AudioManager.instance.PlaySFX(2, player.transform);
            collision.GetComponent<ArrowController>().FlipArrow();
        }

        SetupTargetsForBounce(collision);

       

    }

    private void SwordSkillDamage(Enemy enemy)
    {
        if (enemy.GetComponent<CharacterStats>().isDead) return;
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        //enemy.TakeDamage(10);
        player.stats.DoDamage(enemy.GetComponent<CharacterStats>());
        if (player.skillManager.sword.timeStopUnlocked)
        {
            enemy.FreezeTimer(freezeTimeDuration);
        }
        if(player.skillManager.sword.volnurableUnlocked)
        {
            enemyStats.MakeVulnerableFor(freezeTimeDuration);
        }
        //enemy.StartCoroutine("FreezeTimerFor", freezeTimeDuration);
        ItemDataEquipment equipment = Inventory.instance.GetEquipmentType(EquipmentType.Amulet);
        if (equipment != null)
        {
            equipment.ItemEffect(enemy.transform);
        }
    }

    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null&&collision.GetComponent<CharacterStats>().isDead == false)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
                foreach (var collider in colliders)
                {
                    if (collider.GetComponent<Enemy>() != null && !collider.GetComponent<CharacterStats>().isDead)
                    {
                        enemyTarget.Add(collider.transform);
                    }
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0&& collision.GetComponent<Enemy>() != null&& collision.GetComponent<CharacterStats>().isDead == false)
        {
            pierceAmount--;
            return;
        }
        if (isSpinning && collision.GetComponent<Enemy>() != null && collision.GetComponent<CharacterStats>().isDead == false)
        {
            StopWhenSpinning();
            return;
        }
        
        canRotate = false;
        circleCollider.enabled = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if (isBouncing && enemyTarget.Count > 0) return;


        animator.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
