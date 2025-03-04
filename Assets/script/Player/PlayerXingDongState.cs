using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXingDongState : PlayerState
{
    public PlayerXingDongState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();
        if (!player.IsWallDetected()&& player.skillManager.dash.dashUnlocked && !player.isOnSlope)
        {
            if (InputManager.Instance.canDash )
            {
                InputManager.Instance.canDash = false;
                if (SkillManager.instance.dash.CanUseSkill())
                {
                    player.dashDir = Input.GetAxisRaw("Horizontal");
                    if (player.dashDir == 0)
                    {
                        player.dashDir = player.facingDirection;
                    }

                    stateMachine.ChangeState(player.dashState);
                }
            }
        }
        if (InputManager.Instance.canDistanceAttack && player.skillManager.crystal.crystalUnlocked)
        {
            InputManager.Instance.canDistanceAttack = false;
            //Debug.Log("use crystal");
            
            player.skillManager.crystal.CanUseSkill();
        }
        if ((yInput <= -0.8 && player.IsLadderDetected2() || yInput >= 0.8 && player.IsLadderDetected()))
        {

            stateMachine.ChangeState(player.climbState);
        }
    }
}
