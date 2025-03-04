using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_4BattleState : SkeletonBattleState
{

    private float facingDirection;

    new protected EnemySkeleton_4 enemy;



    public Skeleton_4BattleState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemySkeleton_4;
    }

    public override void Enter()
    {
        base.Enter();
        //stateTimer = 1;
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
       
         base.Update();
        if (AttackEntity == null) return;
        if (((enemy.transform.position.x > AttackEntity.position.x - enemy.jumpChect*0.7f) && 
            (enemy.transform.position.x < AttackEntity.position.x + enemy.jumpChect*0.7f))
            &&enemy.IsGroundedDetected() && !enemy.CheckSq2Ground()&& enemy.transform.position.y < AttackEntity.position.y + 2.5f)
        {
            stateMachine.ChangeState(enemy.jumpState);
        }
       
    }
}
