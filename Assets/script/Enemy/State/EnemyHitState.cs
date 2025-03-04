using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class EnemyHitState : EnemyState
{
    //protected float animspeed;
    protected Enemy enemy;

    public EnemyHitState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy;


    }



    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(-0.1f*PlayerManager.instance.player.facingDirection, rb.velocity.y);
        //animspeed = enemy.anim.speed;
        //enemy.stats.MakeisInvincible(true);
       // enemy.entityFX.InvokeRepeating("RedColorBlink", 0, enemy.knockbackDuration);
        stateTimer = 0.5f;
        // rb.=enemy.SetVelocity(-enemy.facingDirection * 5, 5);
        //rb.velocity=new Vector2(-enemy.facingDirection * enemy.knockback.x, enemy.knockback.y);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.anim.speed = 1;
        enemy.isHitOver = false;
        enemy.isAnimationStop = false;
       
        // enemy.stats.MakeisInvincible(false);
        //enemy.entityFX.Invoke("CancelRedBlink", 0);
    }

    public override void Update()
    {
        base.Update();
        if (enemy.IsGroundedDetected()&&stateTimer<0.2f)
        {
            enemy.isAnimationStop = false;
            if (enemy.isHitOver)
            {
                stateMachine.ChangeState(enemy.idleState);
                enemy.isHitOver = false;
            }

        }
        if (stateTimer < 0 )
        {
            stateMachine.ChangeState(enemy.idleState);
            
        }
        if (enemy.IsGroundedDetected() && enemy.bigHitState.isCanBigHit)
        {
            enemy.entityFX.Invoke("CancelRedBlink", 0);
            stateMachine.ChangeState(enemy.bigHitState);
        }
        if (enemy.isAnimationStop)
        {
            enemy.anim.speed = 0;
        }
        else
        {
            enemy.anim.speed = 1;
        }
    }
}
