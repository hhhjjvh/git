using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemyBringerofDeath : Enemy
{
    public int Teleportamount;
    public bool canTelepor;
    public float castamount;
    public bool canCast;

    public float castTime;
    public virtual BringerofDeathTeleportState teleportState { get;protected set; }
    public virtual BringerofDeathCastState castState { get; protected set; }
    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void Start()
    {
        base.Start();
        enemyName = EnemyName.BringerofDeath;
        idleState = new EnemyIdleState(this, stateMachine, this, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, this, "Move");
        battleState = new BringerofDeathBattleState(this, stateMachine, this, "Move");
        attackState = new BringerofDeathAttackState(this, stateMachine, this, "Attack");
        bigHitState = new EnemyBigHitState(this, stateMachine, this, "BigHit");
        hitState = new BringerofDeathHitState(this, stateMachine, this, "Hit");
        deadState = new EnemyDeadState(this, stateMachine, this, "Dead");
        teleportState = new BringerofDeathTeleportState(this, stateMachine, this, "Teleport");
        castState = new BringerofDeathCastState(this, stateMachine, this, "Cast");
        stateMachine.Initialize(idleState);
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
            if (GetComponent<CharacterStats>().isOverlordBody) return;
            stateMachine.ChangeState(hitState);
        }
    }
    public override bool CanBeStunned()
    {
        base.CanBeStunned();
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
    }
}
