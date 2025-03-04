using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EnemyMoveState : EnemyGroundState
{
    public EnemyMoveState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
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
        float rand= Random.Range(0.7f, 1.3f);
        enemy.SetVelocity(enemy.moveSpeed* enemy.facingDirection*rand, rb.velocity.y);

        if (!enemy.IsGroundedDetected()|| enemy.IsWallDetected()||((enemy.playerChackTransform.transform.position.x <= enemy.StartPosition1&&enemy.facingDirection==-1) 
            || (enemy.playerChackTransform.transform.position.x >= enemy.StartPosition2)&& enemy.facingDirection==1))
        {
            enemy.Flip();
            stateMachine.ChangeState(enemy.idleState);
        }
        
    }
    

}
