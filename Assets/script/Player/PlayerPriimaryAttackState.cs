using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPriimaryAttackState : PlayerGroundedState
{
    private int comboCounter;
    private float  lastAttackTime;
    private float comboWindow =.9f;

    private float grivaty;

    public bool isPriimaryAttack;


    public PlayerPriimaryAttackState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }


    public override void Enter()
    {

        base.Enter();
        //animSpeed = player.anim.speed;
        player.isAttack= true;

       grivaty = rb.gravityScale;

        if (comboCounter > 2 || Time.time - lastAttackTime > comboWindow)
        {
            comboCounter = 0;
        }
        isPriimaryAttack = true;
        xInput = 0;
        if (rb.velocity.y != 0)
        {
            comboCounter = 2;
        }
        player.anim.SetInteger("ComboCounter", comboCounter);
        float attackDirection = player.facingDirection;
        if(xInput!=0)
        {
            attackDirection = xInput;
        }
        player.anim.speed = 0.5f*(comboCounter*0.2f+1);
        player.SetVelocity(player.attackMovement[comboCounter] *attackDirection, rb.velocity.y);
        if (rb.velocity.y != 0)
        {
            rb.gravityScale *=3 ;
        }


        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = grivaty;
        player.isAttack = false;
        player.StartCoroutine("BusyFor", .2f);
        player.anim.speed =1;
        lastAttackTime = Time.time;
        isPriimaryAttack = false;
        player.CloseCounterAttackWindow();
    }

    public override void Update()
    {
        base.Update();
       
        if (stateTimer<0)
        {
            player.SetVelocity(0, rb.velocity.y);
        }
       if(triggerCalled)
        {
            //comboCounter = 0;
            stateMachine.ChangeState(player.idleState);

        }
    }
    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        comboCounter++;
        

    }
    
    public int GetComboCounter()
    {
        return comboCounter;
    }
}
