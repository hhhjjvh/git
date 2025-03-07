using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skillManager.sword.DotsActive(true);
       
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", 0.2f);
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, rb.velocity.y);
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            stateMachine.ChangeState(player.idleState);
        }

        Vector2 mouseposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseposition.x < player.transform.position.x && player.facingDirection == 1)
        {
            player.Flip();
        }
        else if (mouseposition.x > player.transform.position.x && player.facingDirection == -1)
        {
            player.Flip();
        }
    }

}
