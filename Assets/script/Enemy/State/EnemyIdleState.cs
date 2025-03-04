using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyIdleState : EnemyGroundState
{
    
    public EnemyIdleState(Enemy enemybase ,EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine,enemy, animBoolName)
    {
    }



    public override void Enter()
    {
        base.Enter();
        enemy.healthBarUI.gameObject.SetActive(true);
        enemy.anim.speed = 1;
        enemy.isAnimationStop = false;
        stateTimer = Random.Range(0.5f*enemy.idleTime, 1.5f * enemy.idleTime);
        enemy.SetVelocity(0, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.isAnimationStop = false;
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
        enemy.healthBarUI.gameObject.SetActive(true);
        enemy.anim.speed = 1;
        enemy.isAnimationStop = false;
    }
}
