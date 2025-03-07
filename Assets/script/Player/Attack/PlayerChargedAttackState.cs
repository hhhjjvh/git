using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerChargedAttackState : PlayerState
{
    protected int attackLevel;
    protected float damageMultiplier;

    public PlayerChargedAttackState(PlayerStateMachine playerStateMachine, player1 player, string animBoolName, int level)
        : base(playerStateMachine, player, animBoolName)
    {
        attackLevel = level;
        damageMultiplier = player.chargeAttackData.GetDamageMultiplier(level);
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0);
        player.anim.SetInteger("ChargeLevel", attackLevel);
    }
    public override void Exit()
    {
        base.Exit();
        player.CloseCounterAttackWindow();
    }

    public override void Update()
    {
        base.Update();
        player.entityFX.CreatAfterImage();
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    protected virtual void ApplyAttackEffects()
    {
        // 触发攻击轨迹特效
        player.entityFX.StartAttackTrail();
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            player.attackCheck.position,
            player.chargeAttackData.GetAttackRange(attackLevel)
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out var enemy) && !hit.GetComponent<CharacterStats>().isDead)
            {
                player.stats.DoDamage(enemy.GetComponent<CharacterStats>(),
                    damageMultiplier);
                ApplyKnockback(enemy);
                // 在命中点播放冲击特效
                player.entityFX.PlayAttackImpact(hit.ClosestPoint(player.transform.position));
            }
        }
    }

    private void ApplyKnockback(Enemy enemy)
    {
        Vector2 direction = new Vector2(player.facingDirection, 0.5f).normalized;
        float force = player.chargeAttackData.GetKnockbackForce(attackLevel);
       // enemy.MakeKnockback(direction * force, 1f);
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        ApplyAttackEffects();
        player.entityFX.StopAttackTrail();
        //  player.entityFX.CreateChargedAttackEffect(attackLevel);
    }
}
// 具体实现类
public class ChargedAttackLv1 : PlayerChargedAttackState
{
    public ChargedAttackLv1(PlayerStateMachine psm, player1 p)
        : base(psm, p, "ChargeAttack1", 1) { }
   protected override void ApplyAttackEffects()
    {
        base.ApplyAttackEffects();
    }
}

public class ChargedAttackLv2 : PlayerChargedAttackState
{
    public ChargedAttackLv2(PlayerStateMachine psm, player1 p)
        : base(psm, p, "ChargeAttack2", 2) { }
    protected override void ApplyAttackEffects()
    {
        base.ApplyAttackEffects();
      GameObject iceAndFireEffect=  PoolMgr.Instance.GetObj("crossed", player.transform.position, player.transform.rotation);
        iceAndFireEffect.GetComponent<Rigidbody2D>().velocity = new Vector2(15 * player.facingDirection, 0);
       // PoolMgr.Instance.Release(iceAndFireEffect, 1f);
    }
}

public class ChargedAttackLv3 : PlayerChargedAttackState
{
    public ChargedAttackLv3(PlayerStateMachine psm, player1 p)
        : base(psm, p, "ChargeAttack3", 3) { }
    protected override void ApplyAttackEffects()
    {
        base.ApplyAttackEffects();
       
        float yRotation = player.transform.eulerAngles.y;
        Quaternion effectRotation = Quaternion.Euler(0, yRotation+180, 0);
        GameObject ChargeStage3 = PoolMgr.Instance.GetObj("ChargeStage3", player.transform.position, effectRotation);
        ChargeStage3.GetComponent<Rigidbody2D>().velocity = new Vector2(20 * player.facingDirection, 0);
      
    }
}
public class PlayerSuperChargeAttackState : PlayerChargedAttackState
{
    public PlayerSuperChargeAttackState(PlayerStateMachine psm, player1 player)
        : base(psm, player, "SuperChargeAttack", 4) { }

    public override void Enter()
    {
        base.Enter();
      //  player.entityFX.PlaySuperChargeEffect();
     //   CameraManager.Instance.ShakeCamera(0.5f, 0.8f);
    }

    protected override void ApplyAttackEffects()
    {
        // 3倍范围冲击波
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            player.attackCheck.position,
            player.chargeAttackData.GetAttackRange(3) * 3f
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out var enemy))
            {
               // enemy.ApplySuperKnockback();
                player.stats.DoDamage(enemy.stats, 2f);
            }
        }
        AudioManager.instance.PlaySFX(37, null);
      //  AttackSense.instance.HitPause(15);
        float yRotation = player.transform.eulerAngles.y;
        Quaternion effectRotation = Quaternion.Euler(0, yRotation + 180, 0);
        Vector3 pos= new Vector3(player.transform.position.x, player.transform.position.y+2, player.transform.position.z);
        GameObject ChargeStage3 = PoolMgr.Instance.GetObj("ChargeStage4", pos, effectRotation);
        ChargeStage3.GetComponent<Rigidbody2D>().velocity = new Vector2(35 * player.facingDirection, 0);
    }
}