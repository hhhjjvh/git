using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashAttack : PlayerState
{
    float speed;
    public PlayerDashAttack(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }


    public override void Enter()
    {
        base.Enter();
        speed = player.moveSpeed*1.2f;
        player.stats.MakeOverlordBody(true);
        // player.stats.MakeisInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();
      
        player.SetVelocity(0, rb.velocity.y);
        player.stats.MakeOverlordBody(false);
        //player.stats.MakeisInvincible(false);
    }

    public override void Update()
    {
        base.Update();
        if (speed > 3f)
        {
            speed -= Time.deltaTime * player.moveSpeed * 2.1f;
        }
        else
        {
            speed = 0.1f;
        }
       // Debug.Log(speed);
        player.SetVelocity(speed*player.facingDirection, rb.velocity.y);

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);

        }
        player.entityFX.CreatAfterImage();
    }
}