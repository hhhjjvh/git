using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallGrabState : PlayerState
{
    float gravityScale;
    public PlayerWallGrabState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0.2f;
        gravityScale = player.rb.gravityScale;
        player.rb.gravityScale = 0;
        
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = gravityScale;
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, 0);
        if (InputManager.Instance.canJump|| yInput==1&&stateTimer<=0)
        {
            InputManager.Instance.canJump = false;
            stateMachine.ChangeState(player.jumpState);
        }
        if (yInput==-1)
        {
            stateMachine.ChangeState(player.fallState);
            player.rb.velocity = new Vector2(player.rb.velocity.x,-player.jumpForce);
        }
    }
}
