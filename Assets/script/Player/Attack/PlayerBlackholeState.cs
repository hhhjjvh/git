using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime=.4f;
    private bool skillUsed;

    private float defaultGravity;
   
    public PlayerBlackholeState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void Enter()
    {
        base.Enter();
       
        player.stats.MakeisInvincible(true);
        player.anim.speed = 2.5f;
        defaultGravity = player.rb.gravityScale;
        skillUsed = false;
        stateTimer=flyTime;
        rb.gravityScale = 0;
       
    }

    public override void Exit()
    {
        base.Exit();
        player.stats.MakeisInvincible(false);
        player.rb.gravityScale = defaultGravity;
        player.MakeTransprent(false);
        player.anim.speed = 1;
        AudioManager.instance.StopSFX(11);
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer>0)
        {
            rb.velocity=new Vector2(0,15);
        }
        else
        {
            rb.velocity = new Vector2(0,-0.1f);
            if(!skillUsed)
            {
                if (player.skillManager.blackhole.CanUseSkill())
                {
                    skillUsed = true;
                    AudioManager.instance.PlaySFX(11, player.transform);
                }
            }
        }
        if(player.skillManager.blackhole.SkillCompleted())
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}
