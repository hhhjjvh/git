using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{
   

    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void Start()
    {
        base.Start();
        idleState = new EnemyIdleState(this, stateMachine, this, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, this, "Move");
        battleState = new EnemyBattleState(this, stateMachine, this, "Move");
        attackState = new EnemyAttackState(this, stateMachine, this, "Attack");
        bigHitState = new EnemyBigHitState(this, stateMachine, this, "BigHit");
        hitState = new EnemyHitState(this, stateMachine, this, "Hit");
        deadState = new EnemyDeadState(this, stateMachine, this, "Dead");
       
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
            stateMachine.ChangeState(hitState);
        }
    }
    public override bool CanBeStunned()
    {
        if(base.CanBeStunned())
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
