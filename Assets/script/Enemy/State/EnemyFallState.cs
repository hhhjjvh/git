using UnityEngine;

public class EnemyFallState : EnemyBattleState
{
    protected Enemy_Skeleton enemy;
    public EnemyFallState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {

        this.enemy = (Enemy_Skeleton)enemy;

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
        if (enemy.IsGroundedDetected())
        {
            stateMachine.ChangeState(enemy.idleState);
        }
       
    }
}
