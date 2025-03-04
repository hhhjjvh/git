using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_3AttackState : EnemyPriimaryAttackState
{
    new protected Enemy_Skeleton enemy;

    protected float comboWindow = 0.5f;

    public Skeleton_3AttackState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as Enemy_Skeleton;
    }

    public override void Enter()
    {
        base.Enter();
        if (comboCounter > 2 || Time.time - enemy.lastTimeAttacked > comboWindow)
        {
            comboCounter = 0;
        }
        enemy.anim.SetInteger("ComboCounter", comboCounter);
        if (comboCounter == 1)
        {
            enemy.SetVelocity(enemy.moveSpeed * enemy.facingDirection, 10f);
        }
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
        if (comboCounter ==1)
        {
            enemy.SetVelocity(enemy.moveSpeed * enemy.facingDirection, rb.velocity.y);
        }

        if (triggerCalled)
        {
            comboCounter++;
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
