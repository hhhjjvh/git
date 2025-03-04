using UnityEngine;

public class PlayerJumpState : PlayerXingDongState
{
    //public bool isjumping;

    public PlayerJumpState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0.3f;
        player.isJumped = false;
        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);

    }

    public override void Exit()
    {
        base.Exit();
        // player.isJumped = true;

    }

    public override void Update()
    {
        base.Update();
        if (InputManager.Instance.canJump && !player.isJumped && stateTimer < 0)
        {
            InputManager.Instance.canJump = false;
            //Debug.Log("jump");
            rb.velocity = new Vector2(rb.velocity.x, player.jumpForce * 0.9f);
            player.isJumped = true;
        }
        //player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);
        if (rb.velocity.y < -0.01f)
        {
            stateMachine.ChangeState(player.fallState);
        }
        if ((InputManager.Instance.canAttack) && !player.isAttack && yInput < -0.5f && player.GetComponent<PlayerStats>().mana >= 50)
        {
            InputManager.Instance.canAttack = false;
            player.GetComponent<PlayerStats>().mana -= 50;
            stateMachine.ChangeState(player.useSkillWithBigState);
        }
        if ((InputManager.Instance.canAttack) && !player.isAttack)
        {
            InputManager.Instance.canAttack = false;
            //Debug.Log("attack");
            stateMachine.ChangeState(player.primaryAttackState);
        }
        if (xInput != 0)
        {
            player.SetVelocity(xInput * player.moveSpeed * .8f, rb.velocity.y);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            stateMachine.ChangeState(player.flyState);
        }

    }
}
