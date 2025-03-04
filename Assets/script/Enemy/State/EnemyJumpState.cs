using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpState : EnemyState
{
    public float facingDirection;
    protected Enemy enemy;
    public EnemyJumpState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
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
       
    }

    public override void Update()
    {
        base.Update();
        float rand = Random.Range(0.7f, 1.3f);
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDirection * rand, rb.velocity.y);

      
    }
}