using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SlimeType
{
    big,medium,small
}

public class EnemySlime : Enemy
{
    [SerializeField] private SlimeType slimeType;
    [SerializeField] private int slimesToCreate;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Vector2 minCreationVector;
    [SerializeField] private Vector2 maxCreationVector;

    public SlimeCreatState creatState {  get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
    }
    void OnEnable()
    {
        if (anim == null || stateMachine == null || idleState == null) return;

        anim.speed = 1;
        stateMachine.Initialize(creatState);
    }
    protected override void Start()
    {
        base.Start();
        if (slimeType == SlimeType.big)
        {
           enemyName = EnemyName.Slime_big;
        }
        else if (slimeType == SlimeType.medium)
        {
            enemyName = EnemyName.Slime_medium;
        }
        else
        {
            enemyName = EnemyName.Slime_small;
        }
        //enemyName = EnemyName.Slime_small;
         idleState = new EnemyIdleState(this, stateMachine, this, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, this, "Move");
        battleState = new EnemyBattleState(this, stateMachine, this, "Move");
        attackState = new EnemyAttackState(this, stateMachine, this, "Attack");
        bigHitState = new EnemyBigHitState(this, stateMachine, this, "BigHit");
        hitState = new EnemyHitState(this, stateMachine, this, "Hit");
        deadState = new EnemyDeadState(this, stateMachine, this, "Dead");
        creatState = new SlimeCreatState(this, stateMachine, this, "Idle");
        if (slimeType == SlimeType.big)
        {
            stateMachine.Initialize(idleState);
        }
        else
        {
            stateMachine.Initialize(creatState);
        }
    }

    protected override void Update()
    {
        base.Update();
       
    }
    public override void MakeDamageFX(int damage)
    {
        base.MakeDamageFX(damage);
        if (!GetComponent<CharacterStats>().isDead)
        {
            
            stateMachine.ChangeState(hitState);
        }
    }
    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(hitState);
            return true;
        }
        return false;
    }
    public override void Die()
    {
        base.Die();
       
        stateMachine.ChangeState(deadState);
        //Debug.Log(slimeType);
        if(slimeType==SlimeType.small||!canMove) return;
        CreateSlimes(slimesToCreate, slimePrefab);

    }
    private void CreateSlimes(int amount,GameObject prefab)
    {
        //Debug.Log("Create Slime");
        for (int i = 0; i < amount; i++)
        {
            GameObject newSlime;
            if (attackLayerName == "Player")
            {
                if (slimeType == SlimeType.big)
                {
                    newSlime = EnemyFactory.Instance.GetEnemy(EnemyName.Slime_medium, 1, transform.position);
                }
                else if (slimeType == SlimeType.medium)
                {
                    newSlime = EnemyFactory.Instance.GetEnemy(EnemyName.Slime_small, 1, transform.position);
                }
                else
                {
                    return;
                }
                if (RandomMapGenerator.Instance != null && RandomMapGenerator.Instance.currentRoom != null)
                {
                    Room room = RandomMapGenerator.Instance.currentRoom;
                    room.AddEnemy(newSlime);
                    room.EnemyCount++;
                }
            }
            else
            {
                if (slimeType == SlimeType.big)
                {
                    newSlime = EnemyFactory.Instance.GetCharmEnemy(EnemyName.Slime_medium, 1, transform.position);
                }
                else if (slimeType == SlimeType.medium)
                {
                    newSlime = EnemyFactory.Instance.GetCharmEnemy(EnemyName.Slime_small, 1, transform.position);
                }
                else
                {
                    return;
                }
            }
            // Instantiate(prefab, transform.position, Quaternion.identity);
            newSlime.GetComponent<EnemySlime>().SetUpSlime(facingDirection );
            int level = GetComponent<EnemyStats>().GetLevel();
            int newLevel = level / 2+Random.Range(0, level / 2);
            if (newLevel < 1) newLevel = 1;
            newSlime.GetComponent<EnemyStats>().SetLevel(newLevel);
            //var bounds = GameObject.FindGameObjectWithTag("Bounds");
            //if (bounds != null)
            //{
            //    newSlime.transform.SetParent(bounds.transform);
            //}
            newSlime.transform.SetParent(transform.parent);
            
        }
    }
    public void SetUpSlime(int facingDirection)
    {
       // Debug.Log("Set up Slime");
        if(this.facingDirection != facingDirection)
        {
            Flip();
        }
        float xVector = Random.Range(minCreationVector.x, maxCreationVector.x);
        float yVector = Random.Range(minCreationVector.y, maxCreationVector.y);
        isKnocked=true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(xVector*facingDirection, yVector);
        Invoke("CanCelKnockbake", 1.5f);
        
    }
    private void CanCelKnockbake()=>isKnocked = false;
}
