using UnityEngine;

public class SkeletonPriimaryAttackState : EnemyPriimaryAttackState
{
    new protected Enemy_Skeleton enemy;

    protected float comboWindow = 0.5f;

    public SkeletonPriimaryAttackState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(enemybase, stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as Enemy_Skeleton;
    }

    public override void Enter()
    {
        base.Enter();
        if (comboCounter > 1 || Time.time - enemy.lastTimeAttacked > comboWindow)
        {
            comboCounter = 0;
        }
        enemy.anim.SetInteger("ComboCounter", comboCounter);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {

            comboCounter++;

            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
