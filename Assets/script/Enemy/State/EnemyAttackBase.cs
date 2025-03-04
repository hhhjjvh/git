using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBase : EnemyState
{
    public int comboCounter;
    protected Enemy enemy;
    public EnemyAttackBase(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy;


    }



    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastTimeAttacked = Time.time;
        enemy.CloseCounterAttackWindow();
    }

    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(0, rb.velocity.y);
        
    }
}
