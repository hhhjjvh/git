using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreChargeState : PlayerState
{
    private float pressStartTime;
    private const float ChargeThreshold = 0.2f;

    public PlayerPreChargeState(PlayerStateMachine psm, player1 player)
        : base(psm, player, "Idle") { }

    public override void Enter()
    {
        base.Enter();
        pressStartTime = Time.time;
        player.SetVelocity(0, 0); // 禁止移动
    }

    public override void Update()
    {
        base.Update();

        // 持续检测输入状态
        if (InputManager.Instance.attackButtonUp)
        {
            // 短按触发普通攻击
            if (Time.time - pressStartTime < ChargeThreshold)
            {
                stateMachine.ChangeState(player.primaryAttackState);
            }
            // 长按后释放进入蓄力攻击
            else
            {
                stateMachine.ChangeState(player.chargeAttackState);
            }
        }
        // 超时自动进入蓄力状态
        else if (Time.time - pressStartTime >= ChargeThreshold)
        {
            stateMachine.ChangeState(player.chargeAttackState);
        }

        // 被打断则退出
        if (!player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}