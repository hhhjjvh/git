using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlyState : PlayerState
{
    private Vector2 colliderOffset;
    private Vector2 colliderSize;
    private float gravityScale;
    public PlayerFlyState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        gravityScale = player.rb.gravityScale;
        player.rb.gravityScale = 0;

        colliderOffset = player.cd.offset;
        colliderSize = player.cd.size;
        
        player.cd.offset = new Vector2(0.5f, -0.2f);
        player.cd.size = new Vector2(0.51f,0.5f);

        stateTimer = 0.3f;
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = gravityScale;
        player.cd.offset = colliderOffset;
        player.cd.size = colliderSize;
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(xInput * player.flySpeed, yInput * player.flySpeed);
        if (Input.GetKeyDown(KeyCode.O))
        {
            stateMachine.ChangeState(player.fallState);
        }
        if (stateTimer < 0)
        {
            stateTimer= 0.05f;
            player.GetComponent<PlayerStats>().mana--;
            if (player.GetComponent<PlayerStats>().mana <= 0)
            {
                stateMachine.ChangeState(player.fallState);
            }

        }
        //player.entityFX.CreatAfterImage();
    }
}
