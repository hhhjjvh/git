using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerofDeathHitState : EnemyHitState
{
    new EnemyBringerofDeath enemy;
    public BringerofDeathHitState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemyBringerofDeath;
    }

    public override void Enter()
    {
        base.Enter();
        if (Random.Range(0, 99) < enemy.Teleportamount)
        {
            enemy.canTelepor = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        if (enemy.canTelepor)
        {
            stateMachine.ChangeState(enemy.teleportState);
        }
        base.Update();
    }
}
