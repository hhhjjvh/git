using System.Collections;
using UnityEngine;


public class SkeletonBattleState : EnemyBattleState
{
    protected EnemySkeleton_1 enemy;
    public SkeletonBattleState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemySkeleton_1;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(AttackEntity == null) return;
      //  if (AttackEntity.GetComponent<player1>() != null)
        {
            if (!enemy.CheckSqGround() && enemy.transform.position.y < AttackEntity.position.y
            && enemy.IsGroundedDetected() && AttackEntity.GetComponent<entity>().IsGroundedDetected())
            {
                stateMachine.ChangeState(enemy.jumpState);
            }
            if (!enemy.CheckSqGround() &&enemy.IsWallDetected()
           && enemy.IsGroundedDetected() && AttackEntity.GetComponent<entity>().IsGroundedDetected())
            {
                stateMachine.ChangeState(enemy.jumpState);
            }
        }
    }
}
