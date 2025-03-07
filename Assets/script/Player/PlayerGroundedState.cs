using UnityEngine;

public class PlayerGroundedState : PlayerXingDongState
{
    private float attackPressTime;
    public PlayerGroundedState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
     //  attackPressTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();
        if (!player.IsGroundedDetected() && !player.isAttack && !player.isOnSlope)
        {
            if (stateMachine.currentState != player.runState)
            {
                stateMachine.ChangeState(player.fallState);
            }
        }
        if (InputManager.Instance.canJump && player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.jumpState);
            InputManager.Instance.canJump = false;
        }
        // 短按攻击处理（修改原蓄力条件）
        if (InputManager.Instance.attackButtonDown)
        {
            attackPressTime = Time.time;
        }

        //if (InputManager.Instance.attackButtonUp && !player.isAttack)
        //{
        //    InputManager.Instance.attackButtonUp = false;
        //    float holdDuration = Time.time - attackPressTime;
        //    Debug.Log("Hold duration: " + holdDuration);
        //    if (holdDuration < 0.2f) // 短按触发普通攻击
        //    {
        //        InputManager.Instance.canAttack = false;

        //        stateMachine.ChangeState(player.primaryAttackState);
        //    }
        //    else //if (player.IsGroundedDetected()) // 长按触发蓄力
        //    {
        //        Debug.Log("Charge attack");
        //        stateMachine.ChangeState(player.chargeAttackState);
        //    }
        //}
        //        if (InputManager.Instance.attackButtonHold && player.IsGroundedDetected() && !player.isAttack)
        //{
        //            // 按住攻击键进入蓄力状态
        //            stateMachine.ChangeState(player.chargeAttackState);
        //            return;
        //        }
        if ((InputManager.Instance.canAttack) && !player.isAttack)
        {

            InputManager.Instance.canAttack = false;
            stateMachine.ChangeState(player.preChargeState);
        }

        if (InputManager.Instance.canParry)
        {
            InputManager.Instance.canParry = false;
            if (player.skillManager.parry.parryUnlocked && player.skillManager.parry.cooldownTimer <= 0)
            {
                stateMachine.ChangeState(player.counterAttackState);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && HasNoSword() && player.skillManager.sword.swordUnlocked && player.skillManager.sword.CanUseSkill())
        {
            stateMachine.ChangeState(player.aimSwordState);
            //PlayerManager.instance.player.GetComponent<PlayerStats>().mana -= 10;
        }
        if (InputManager.Instance.canUseSkill && player.skillManager.blackhole.blackholeUnlocked)
        {
            InputManager.Instance.canUseSkill = false;
            if (player.skillManager.blackhole.cooldownTimer <= 0)
            {
                if (player.GetComponent<PlayerStats>().mana < player.skillManager.blackhole.needMana)
                {
                    player.entityFX.CreatePopUpText("魔力不足", Color.red);
                    AudioManager.instance.PlaySFX(17, null);
                }
                else
                {
                    AudioManager.instance.PlaySFX(10, player.transform);
                    stateMachine.ChangeState(player.blackholeState);
                }
            }
            else
            {
                player.entityFX.CreatePopUpText("技能冷却中", Color.red);
                AudioManager.instance.PlaySFX(17, null);
            }
        }
        if ((yInput <= -0.8 && player.IsLadderDetected2()))
        {

            stateMachine.ChangeState(player.climbState);
        }
        if ((yInput <= -0.8 && player.IsGroundedDetected()) || player.IsWallDetected3())
        {
            stateMachine.ChangeState(player.croushState);
        }
        if (yInput >= 0.8 && player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.parryState);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            stateMachine.ChangeState(player.flyState);
        }


    }

    private bool HasNoSword()
    {
        if (player.sword == false)
        {
            return true;
        }
        player.sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }
}
