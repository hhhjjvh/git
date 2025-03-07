using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCathSwordState : PlayerState
{
    private Transform swordTransform;
    public PlayerCathSwordState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.entityFX.ScreenShake(player.entityFX.shakeSwordPower);
        if (player.sword != null)
        {
            swordTransform = player.sword.transform;
            if (swordTransform.position.x < player.transform.position.x && player.facingDirection == 1)
            {
                player.Flip();
            }
            else if (swordTransform.position.x > player.transform.position.x && player.facingDirection == -1)
            {
                player.Flip();
            }
            rb.velocity = new Vector2(-player.facingDirection * player.swordReturnImpact, rb.velocity.y);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.2f);
    }

    public override void Update()
    {
        base.Update();
        if(triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
