using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Player/ChargeAttack")]
public class ChargeAttackData : ScriptableObject
{
    [Header("Timing Settings")]
    public float minChargeTime = 0.5f;
    public float maxChargeTime = 2f;
    public float midTime = 1.0f;    // 二级蓄力时间
    public float maxTime = 2.0f;    // 三级蓄力时间

    [Header("Damage Settings")]
    public float[] damageMultipliers = { 1.2f, 1.5f, 2f };
    [Header("Super Charge Settings")]
    public float superChargeMultiplier = 2.5f;
    public float superChargeRangeMultiplier = 3f;

    [Header("Attack Range")]
    public float[] attackRanges = { 1f, 1.5f, 2f };

    [Header("Knockback")]
    public float[] knockbackForces = { 5f, 8f, 12f };

    public float GetDamageMultiplier(int level)
    {
        return damageMultipliers[Mathf.Clamp(level - 1, 0, damageMultipliers.Length - 1)];
    }

    public float GetAttackRange(int level)
    {
        return attackRanges[Mathf.Clamp(level - 1, 0, attackRanges.Length - 1)];
    }

    public float GetKnockbackForce(int level)
    {
        return knockbackForces[Mathf.Clamp(level - 1, 0, knockbackForces.Length - 1)];
    }
}
