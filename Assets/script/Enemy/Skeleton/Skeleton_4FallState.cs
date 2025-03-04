using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_4FallState : EnemyFallState
{
   
   new protected EnemySkeleton_4 enemy;
    public Skeleton_4FallState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {

        this.enemy = (EnemySkeleton_4)enemy;


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
       // base.Update();
        if (enemy.IsGroundedDetected())
        {
            stateMachine.ChangeState(enemy.idleState);
        }

        float rand = Random.Range(0.7f, 1.3f);

        enemy.rb.velocity = new Vector2(enemy.moveSpeed * rand *enemy.jumpState.facingDirection * enemy.facingDirection*0.5f, rb.velocity.y);

    }
}
