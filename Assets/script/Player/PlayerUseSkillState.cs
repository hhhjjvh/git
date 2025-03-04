using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerUseSkillState : PlayerState
{
    public PlayerUseSkillState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0.7f;
        player.stats.MakeisInvincible(true);
       
    }

    public override void Exit()
    {
        base.Exit();
        player.stats.MakeisInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer<=0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        player.SetVelocity(0, rb.velocity.y);
    }

}