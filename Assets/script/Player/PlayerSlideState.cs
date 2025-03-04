using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideState : PlayerState
{
    //private float slideSpeed = 10f;
    private Vector2 colliderOffset;
    private Vector2 colliderSize;
    public PlayerSlideState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        colliderOffset = player.cd.offset;
        colliderSize = player.cd.size;
        stateTimer = player.slideDuration;
        player.cd.offset = new Vector2(player.cd.offset.x, -0.99f);
        player.cd.size = new Vector2(player.cd.size.x*0.5f, player.cd.size.y * 0.1f);
    }

    public override void Exit()
    {
        base.Exit();
        player.cd.offset = colliderOffset;
        player.cd.size = colliderSize;
    }

    public override void Update()
    {
        base.Update();
      
       
        if (!player.IsGroundedDetected() && (player.IsWallDetected()|| player.IsWallDetected2()))
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        player.SetVelocity(player.slideSpeed * player.facingDirection, rb.velocity.y);
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        player.entityFX.CreatAfterImage();
    }
}