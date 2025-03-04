using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCreatState : EnemyState
{
    protected EnemySlime enemy;
    //protected Transform player;
    public SlimeCreatState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemySlime;


    }



    public override void Enter()
    {
        base.Enter();
        enemy.stats.MakeisInvincible(true);
        stateTimer =1f;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.stats.MakeisInvincible(false);
    }

    public override void Update()
    {
        base.Update();
       // enemy.stats.MakeisInvincible(true);
        if (enemy.IsGroundedDetected() && enemy.rb.velocity.y <= 0|| stateTimer<=0)
        {
            
            stateMachine.ChangeState(enemy.idleState);
        }
       
     
    }
}
