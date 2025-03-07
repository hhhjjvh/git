using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargeAttackState : PlayerState
{
    private float chargeDuration;
    private bool isCharging;
    private bool hasReleasedKey;
    private float chargeProgress;
    private int currentChargeStage;
    // ��������������ر���
    private float superChargeWindowStart;
    private float superChargeWindowDuration = 0.2f;
    public bool superChargeAvailable;
    public PlayerChargeAttackState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName)
        : base(playerStateMachine, player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        chargeDuration = 0f;
        isCharging = true;
        hasReleasedKey = false;
        player.SetVelocity(0, 0); // ��ֹ�ƶ�
        player.anim.SetFloat("ChargeProgress", 0);
        player.stats.MakeOverlordBody(true);
        UI.instance.ChargeUI.ShowUI();
        currentChargeStage = 1;
        player.entityFX.StartChargeEffect(currentChargeStage);
        GenerateSuperChargeWindow();
        // player.entityFX.StartChargeEffect(); // ����������Ч
    }
    private void GenerateSuperChargeWindow()
    {
        // �ڵ���������ʱ������������0.1�봰��
        float thirdStageDuration = player.chargeAttackData.maxTime - player.chargeAttackData.midTime;
        superChargeWindowStart = player.chargeAttackData.midTime +
            Random.Range(thirdStageDuration * 0.3f, thirdStageDuration * 0.7f);
    }
    public override void Exit()
    {
        base.Exit();
        player.stats.MakeOverlordBody(false);
        // player.entityFX.StopChargeEffect(); // ֹͣ������Ч
        player.anim.SetFloat("ChargeProgress", 0);
        UI.instance.ChargeUI.HideUI();
        player.entityFX.StopChargeEffect();
    }

    public override void Update()
    {
        base.Update();

        //HandleChargingInput();
        //UpdateChargeProgress();
        //HandleChargeCompletion();
        //HandleInterruptions();
        if (InputManager.Instance.attackButtonHold)
        {
            chargeDuration += Time.deltaTime;
            UpdateChargeVisuals(); // ����������Ч
        }
        else
        {
            ExecuteChargeAttack(); // �ɿ�����ִ�й���
        }
        CheckSuperChargeWindow();
      //  UpdateSuperChargeUI();

    }
    private void CheckSuperChargeWindow()
    {
        float currentTime = chargeDuration - player.chargeAttackData.midTime;

        // ���볬����������
        if (currentTime >= superChargeWindowStart &&
            currentTime <= superChargeWindowStart + superChargeWindowDuration)
        {
            superChargeAvailable = true;
            UI.instance.ChargeUI.PulseSuperChargeWarning();
        }
        else
        {
            superChargeAvailable = false;
        }

        // �ڴ��������ͷŰ���
        if (InputManager.Instance.attackButtonUp && superChargeAvailable)
        {
            stateMachine.ChangeState(player.superChargeAttackState);
        }
    }

private void UpdateChargeVisuals()
    {
        // ����chargeDuration���¶�������Ч
        float progress = Mathf.Clamp01(chargeDuration / player.chargeAttackData.maxChargeTime);
        player.anim.SetFloat("ChargeProgress", progress);
        int stage = GetChargeStage(progress);

        // ����UI
        UI.instance.ChargeUI.UpdateChargeUI(progress, stage);
        if (stage != currentChargeStage)
        {
            currentChargeStage = stage;
            player.entityFX.StartChargeEffect(currentChargeStage);

            if (currentChargeStage == 3)
                player.entityFX.PlayChargeCompleteEffect();
        }
        //if (progress > 0.8f)
        //    player.entityFX.UpdateChargeEffect(3);
        //else if (progress > 0.4f)
        //    player.entityFX.UpdateChargeEffect(2);
    }
    private int GetChargeStage(float progress)
    {
        if (progress >= 0.8f) return 3;
        if (progress >= 0.4f) return 2;
        return 1;
    }

    private void ExecuteChargeAttack()
    {
        // ��������ʱ��ѡ�񹥻��ȼ�
        if (chargeDuration >= player.chargeAttackData.maxTime)
            stateMachine.ChangeState(player.chargedAttackStateLv3);
        else if (chargeDuration >= player.chargeAttackData.midTime)
            stateMachine.ChangeState(player.chargedAttackStateLv2);
        else
            stateMachine.ChangeState(player.chargedAttackStateLv1);
    }

    private void HandleChargingInput()
    {
        if (!InputManager.Instance.attackButtonUp && !hasReleasedKey)
        {
            hasReleasedKey = true;
            chargeDuration = Mathf.Clamp(chargeDuration, 0, player.chargeAttackData.maxChargeTime);
        }

        if (InputManager.Instance.attackButtonHold)
        {
            chargeDuration += Time.deltaTime;
        }
    }

    private void UpdateChargeProgress()
    {
        chargeProgress = Mathf.Clamp01(chargeDuration / player.chargeAttackData.maxChargeTime);
        player.anim.SetFloat("ChargeProgress", chargeProgress);

        // ���������׶θ�����Ч
       // if (chargeProgress > 0.8f) player.entityFX.UpdateChargeEffect(2);
      //  else if (chargeProgress > 0.4f) player.entityFX.UpdateChargeEffect(1);
    }

    private void HandleChargeCompletion()
    {
        if (!hasReleasedKey) return;

        // ��������ʱ���л���ͬ����״̬
        if (chargeDuration >= player.chargeAttackData.minChargeTime)
        {
            if (chargeProgress >= 0.8f)
            {
                stateMachine.ChangeState(player.chargedAttackStateLv3);
            }
            else if (chargeProgress >= 0.4f)
            {
                stateMachine.ChangeState(player.chargedAttackStateLv2);
            }
            else
            {
                stateMachine.ChangeState(player.chargedAttackStateLv1);
            }
        }
        else
        {
            Debug.Log("Charge time: " + chargeDuration);
            // ��������ص�����״̬
            stateMachine.ChangeState(player.primaryAttackState);
        }
    }

    private void HandleInterruptions()
    {
        if (!player.IsGroundedDetected())// || player.isHurt)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        // ��������״̬
        InputManager.Instance.canAttack = false;
    }
    }
