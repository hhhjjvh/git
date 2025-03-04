using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        AudioManager.instance.PlaySFX(12, player.transform);
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopSFX(12);
    }

    public override void Update()
    {
        if (InputManager.Instance.canAttack)
        {
            InputManager.Instance.canAttack = false;
            stateMachine.ChangeState(player.dashAttackState);
        }
        base.Update();
        if (player.isOnSlope && !player.isJumped)
        {
            // 沿斜坡切线方向移动
            player.SetVelocity(
                xInput * player.moveSpeed * player.slopeNormalPerp.x * -1*1.7f,
                player.rb.velocity.y
            );
        }
        else
        {
            player.SetVelocity(xInput * player.moveSpeed*1.7f, player.rb.velocity.y);
        }
        if (xInput == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        player.entityFX.CreatAfterImage();
    }
}
