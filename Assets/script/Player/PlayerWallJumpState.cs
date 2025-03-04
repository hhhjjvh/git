using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = .4f;
        player.SetVelocity(5*-player.facingDirection, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer <0)
        {
            stateMachine.ChangeState(player.fallState);
        }
        if(player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (xInput != 0)
        {
            player.SetVelocity(xInput * player.moveSpeed * .8f, rb.velocity.y);
        }
    }
}
