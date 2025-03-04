using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    float gravityScale;
    float time;
    
    public PlayerWallSlideState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.anim.transform.position=new Vector3(player.transform.position.x+0.6f*player.facingDirection, player.anim.transform.position.y, 0);
       gravityScale = player.rb.gravityScale;
        player.rb.gravityScale = 0;
        player.rb.velocity = new Vector2(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.transform.position = new Vector3(player.transform.position.x + 0.4f*player.facingDirection, player.anim.transform.position.y, 0);
        player.rb.gravityScale = gravityScale;
    }

    public override void Update()
    {
        base.Update();
        if (!player.IsairDetected() && (player.IsWallDetected() && player.IsWallDetected2()))
        {
            stateMachine.ChangeState(player.wallGrabState);
        }
        if (yInput>=0.6f)
        {
            stateMachine.ChangeState(player.wallGrabState);
        }

        if (!player.IsWallDetected()&&!player.IsWallDetected2())
        {
            stateMachine.ChangeState(player.fallState);

        }
        if (InputManager.Instance.canJump)
        {
            InputManager.Instance.canJump = false;
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        if (xInput != 0 && player.facingDirection != xInput|| player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (yInput < 0)
        {
            time+= Time.deltaTime;
            if (time >0.5f) time = 0.5f;
            player.rb.gravityScale = gravityScale;
            rb.velocity = new Vector2(0, rb.velocity.y * (time+0.5f));
        }
        else
        {
            time = 0;
            //player.rb.gravityScale = 0;
            rb.velocity = new Vector2(0, 0);
        }
       
    }
}