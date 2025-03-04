
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BringerofDeathGroundState : EnemyIdleState
{
    public BringerofDeathGroundState(global::Enemy enemybase, EnemyStateMachine stateMachine, global::Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
    }


    public override void Enter()
    {
        // base.Enter();
        
    }

    public override void Exit()
    {
        // base.Exit();
    }
    public override void Update()
    {
        if (enemy.CheckPlayerInFront() || Vector2.Distance(AttackEntity.transform.position, enemy.transform.position) < 2 || enemy.CheckPlayerInCircle())
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }


}
