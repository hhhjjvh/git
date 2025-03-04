using UnityEngine;

public class PlayerClimbState : PlayerState
{
    private float gravityScale;
    public PlayerClimbState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0.2f;
        gravityScale = player.rb.gravityScale;
        player.rb.gravityScale = 0;
        //CameraControl.Instance.virtualCamera.enabled = false;
        Vector3 pos =//player.transform.position;
             GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        player.gameObject.layer = LayerMask.NameToLayer("Deadly");
        CameraControl.Instance.SwitchLayer(pos);

    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = gravityScale;
        // CameraControl.Instance.virtualCamera.enabled = false;
        Vector3 pos =// player.transform.position;
         GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        player.gameObject.layer = LayerMask.NameToLayer("Player");
        CameraControl.Instance.SwitchLayer(pos);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput * player.moveSpeed * .2f, yInput * player.climbSpeed);
        if (xInput == 0 && yInput == 0)
        {
            player.anim.speed = 0;
        }
        else
        {
            player.anim.speed = 1;
        }
        if (InputManager.Instance.canJump)
        {
            InputManager.Instance.canJump = false;
            stateMachine.ChangeState(player.jumpState);
        }
        if (!player.IsLadderDetected3() && stateTimer <= 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        //if (!((yInput <= -0.8 && player.IsLadderDetected2() || yInput >= 0.8 && player.IsLadderDetected())) && stateTimer <= 0)
        //{
        //    stateMachine.ChangeState(player.idleState);
        //}


        if (((yInput < -0.8 && player.IsGroundedDetected() && !player.IsOneWayPlatformDetected())
        || (yInput > 0.8 && player.IsOneWayPlatformDetected())) && stateTimer <= 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
