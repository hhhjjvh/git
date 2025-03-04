using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_4JumpState : SkeletonJumpState
{
   
    new protected EnemySkeleton_4 enemy;
    public Skeleton_4JumpState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemySkeleton_4;
    }
   


    public override void Enter()
    {
        base.Enter();
        if ((enemy.transform.position.x < AttackEntity.position.x -enemy.jumpChect) || (enemy.transform.position.x > AttackEntity.position.x + enemy.jumpChect))
        {
            facingDirection = 1;
        }
        else
        {
            facingDirection = -1;
        }
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        if (rb.velocity.y < -0.01f)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        base.Update();
        float rand = Random.Range(0.7f, 1.3f);

        //enemy.SetVelocity(enemy.moveSpeed  * rand*facingDirection * enemy.facingDirection, rb.velocity.y);
        enemy.rb.velocity = new Vector2(enemy.moveSpeed * rand * facingDirection * enemy.facingDirection, rb.velocity.y);

    }
}
