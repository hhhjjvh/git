
using UnityEngine;

public class BringerofDeathCastState : EnemyState
{
    EnemyBringerofDeath enemy;

    public BringerofDeathCastState(Enemy enemybase, EnemyStateMachine stateMachine, Enemy enemy, string animBoolName) : base(stateMachine, enemy, animBoolName)
    {
        this.enemy = enemy as EnemyBringerofDeath;

    }

    public override void Enter()
    {
        base.Enter();
     
        // enemy.stats.MakeisInvincible(true);
        enemy.stats.MakeOverlordBody(true);
        enemy.castamount = 0;
        enemy.canCast = false;
        stateTimer = enemy.castTime;
    }

    public override void Exit()
    {
        base.Exit();
        //enemy.stats.MakeisInvincible(false);
        enemy.stats.MakeOverlordBody(false);
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(enemy.teleportState);
        }
        enemy.SetVelocity(0, rb.velocity.y);
    }
}
