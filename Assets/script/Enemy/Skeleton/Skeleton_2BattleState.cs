using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_2BattleState : SkeletonBattleState
{
    new protected EnemySkeleton_2 enemy;
    
    public Skeleton_2BattleState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemySkeleton_2;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 1;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (AttackEntity == null) return;
        if (stateTimer < 0)
        {
            stateTimer = 1;
            if (Vector2.Distance(AttackEntity.transform.position, enemy.transform.position) < enemy.parryDistance)
            {
                stateMachine.ChangeState(enemy.parryState);
            }
        }
    }
}
