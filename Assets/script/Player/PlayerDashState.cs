using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState :PlayerState
{
   
    public PlayerDashState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }


    public override void Enter()
    {
        base.Enter();
        //float offset = Random.Range(-0.5f, 0.5f);
        //SkillManager.instance.clone.CreateClone(player.transform,new Vector2(offset,0));
        player.skillManager.dash.CreateCloneOnDashStart();
        stateTimer = player.dashDuration;

        player.stats.MakeisInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.skillManager.dash.CreateCloneOnDashEnd();
        player.SetVelocity(0, rb.velocity.y);
        player.stats.MakeisInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        if(!player.IsGroundedDetected()&& (player.IsWallDetected() || player.IsWallDetected2()))
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);
       if(stateTimer <0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (InputManager.Instance.canAttack)
        {
            InputManager.Instance.canAttack = false;
            stateMachine.ChangeState(player.dashAttackState);
        }
        player.entityFX.CreatAfterImage();
    }
}
