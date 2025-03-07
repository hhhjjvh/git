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
        player.SetVelocity(0, 0); // ��ֹ�ƶ�
    }

    public override void Update()
    {
        base.Update();

        // �����������״̬
        if (InputManager.Instance.attackButtonUp)
        {
            // �̰�������ͨ����
            if (Time.time - pressStartTime < ChargeThreshold)
            {
                stateMachine.ChangeState(player.primaryAttackState);
            }
            // �������ͷŽ�����������
            else
            {
                stateMachine.ChangeState(player.chargeAttackState);
            }
        }
        // ��ʱ�Զ���������״̬
        else if (Time.time - pressStartTime >= ChargeThreshold)
        {
            stateMachine.ChangeState(player.chargeAttackState);
        }

        // ��������˳�
        if (!player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}