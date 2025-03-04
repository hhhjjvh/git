using System.Collections;
using UnityEngine;

// AddGoldEffect.cs
[CreateAssetMenu(menuName = "Event/Effects/Add Gold")]
public class AddGoldEffect : EventEffect
{
    [SerializeField] int minAmount = 10;
    [SerializeField] int maxAmount = 10;

    public int exp;

    public override void ApplyEffect(GameObject target = null)
    {
        int amount = Random.Range(minAmount, maxAmount + 1);
        PlayerManager.instance.addCoin(amount);
        PlayerManager.instance.player.GetComponent<PlayerStats>().AddExperience(exp);
    }
    public override string GetEffectDescription()
    {
        if (minAmount == maxAmount)
            return $"金币:{minAmount}  经验:{exp}";
        return $"金币:{minAmount}-{maxAmount}  经验:{exp}";
    }
}