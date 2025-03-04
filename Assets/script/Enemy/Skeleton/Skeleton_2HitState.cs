using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_2HitState : EnemyHitState
{
    public bool canParry = false;
   public int parryCount ;

    new protected EnemySkeleton_2 enemy;

    public Skeleton_2HitState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemySkeleton_2;
    }

    public override void Enter()
    {
        base.Enter();
        parryCount += 10;
        if(Random.Range(0, 100) < parryCount)
        {
            canParry = true;
        }

    }

    public override void Exit()
    {
        base.Exit();
        
    }

    public override void Update()
    {
        base.Update();
        if (canParry)
        {
            parryCount = 10;
            canParry = false;
            enemy.isHitOver = true;
            stateMachine.ChangeState(enemy.parryState);
        }
    }
}
