using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerXingDongState
{
    public PlayerAirState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0.1f;
        
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, rb.velocity.y);

        
    }

    public override void Update()
    {
        base.Update();
        if ((InputManager.Instance.canAttack) && !player.isAttack && yInput < -0.5f && player.GetComponent<PlayerStats>().mana >= 50)
        {
            InputManager.Instance.canAttack = false;
            player.GetComponent<PlayerStats>().mana -= 50;
            stateMachine.ChangeState(player.useSkillWithBigState);
        }
        //Debug.Log(player.isJumped);
        if ((InputManager.Instance.canAttack) && !player.isAttack)
        {
            InputManager.Instance.canAttack = false;
            stateMachine.ChangeState(player.primaryAttackState);
        }
        if (InputManager.Instance.canJump && !player.isJumped&&stateTimer<0)
        {
            InputManager.Instance.canJump = false;
            rb.velocity = new Vector2(rb.velocity.x, player.jumpForce * 0.9f);
            player.isJumped = true;
        }
        if (player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
        if((player.IsWallDetected() ||player.IsWallDetected2()))
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
        if(xInput!=0)
        {
            player.SetVelocity(xInput * player.moveSpeed*.8f, rb.velocity.y);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            stateMachine.ChangeState(player.flyState);
        }
    }
}
