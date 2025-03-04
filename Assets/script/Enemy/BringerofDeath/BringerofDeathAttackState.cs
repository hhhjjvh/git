using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerofDeathAttackState : EnemyAttackState
{
    public BringerofDeathAttackState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
    }

    



    public override void Enter()
    {
        base.Enter();
        enemy.anim.speed = 1.2f;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.anim.speed = 1;
    }

    public override void Update()
    {
        base.Update();
    }
}
