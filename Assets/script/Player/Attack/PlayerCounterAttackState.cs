using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
   
    public PlayerCounterAttackState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
       
        stateTimer = player.counterattackDuration;
        player.anim.SetBool("SuccessfuiContterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
        
    }

    public override void Update()
    {
        base.Update();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                if(hit.GetComponent<Enemy>().CanBeStunned())
                {
                    AttackSense.instance.HitPause(15);
                    stateTimer = 10;
                    player.anim.SetBool("SuccessfuiContterAttack", true);
                    player.skillManager.parry.UseSkill();
                    AudioManager.instance.PlaySFX(1, player.transform);
                    //player.skillManager.clone.CreateCloneOnCounterAttack(hit.transform);
                    player.skillManager.parry.MakeMirageOnParry(hit.transform);
                    player.stats.DoDamage(hit.GetComponent<CharacterStats>(),1.5f);
                    hit.GetComponent<Enemy>().MakeKnockbake(new Vector2(5, 13), 1.2f);
                    hit.GetComponent<Enemy>().bigHitState.setBigHit(true);
                }
            }
            if (hit.GetComponent<ArrowController>() != null)
            {
                AudioManager.instance.PlaySFX(2, player.transform);
                stateTimer = 10;
                player.anim.SetBool("SuccessfuiContterAttack", true);
                player.skillManager.parry.UseSkill();
                hit.GetComponent<ArrowController>().FlipArrow();
            }
        }
        if (stateTimer <= 0||triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
