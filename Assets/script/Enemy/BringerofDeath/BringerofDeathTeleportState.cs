
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BringerofDeathTeleportState : EnemyState
{
    protected EnemyBringerofDeath enemy;

    public BringerofDeathTeleportState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemyBringerofDeath;



    }

    public override void Enter()
    {

        base.Enter();
        stateTimer = 3f;
        // enemy.stats.MakeisInvincible(true);
        enemy.stats.MakeOverlordBody(true);
        // enemy.healthBarUI.gameObject.SetActive(false);
        enemy.canTelepor = false;
        enemy.Teleportamount = 20;
        enemy.anim.speed = 2;

        enemy.castamount +=5;
        if (Random.Range(0, 299) < enemy.castamount)
        {
            enemy.canCast = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        //enemy.stats.MakeisInvincible(false);
        enemy.stats.MakeOverlordBody(false);
       // enemy.healthBarUI.gameObject.SetActive(true);
        enemy.anim.speed = 1;
    }

    public override void Update()
    {
        base.Update();
        if(enemy.canCast)
        {
            stateMachine.ChangeState(enemy.castState);
        }


        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
        enemy.SetVelocity(0, rb.velocity.y);
        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
