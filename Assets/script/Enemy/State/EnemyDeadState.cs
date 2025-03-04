using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyDeadState : EnemyState
{
    protected Enemy enemy;

    public EnemyDeadState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy;


    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void Enter()
    {
        base.Enter();
        enemy.DestroyMySelf();
        enemy.healthBarUI.gameObject.SetActive(false);
       // enemy.anim.SetBool(enemy.lastAnimBoolName, true);
       // enemy.isDead = true;
       // Debug.Log("dead");
        //enemy.anim.speed = 0;
        //enemy.cd.enabled= false;
        stateTimer = 1.1f;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.healthBarUI.gameObject.SetActive(true);
        enemy.isAnimationStop = false;
    }

    public override void Update()
    {
        base.Update();
        enemy.SetVelocity(0, rb.velocity.y);
        if (stateTimer > 0)
        {
            stateTimer -= Time.deltaTime;
            rb.velocity = new Vector2(1, 1);
        }
      // Debug.Log(stateTimer);
       
    }
}
