using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParryState : EnemyBattleState
{
    protected EnemySkeleton_2 enemy;
    public EnemyParryState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = (EnemySkeleton_2)enemy;

    }




    public override void Enter()
    {
        base.Enter();
        enemy.isParry = true;
        stateTimer = 3f;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.isParry = false;

    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        //if (stateTimer>0) return;
        //base.Update();
        enemy.SetVelocity(0, rb.velocity.y);
        if (AttackEntity!=null&&(Vector2.Distance(AttackEntity.transform.position, enemy.transform.position) >enemy.parryDistance|| stateTimer <0))
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
