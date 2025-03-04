using UnityEngine;
[CreateAssetMenu(menuName = "Event/Effects/Remove Gold")]
public class RemoveCoinEffect : EventEffect
{
    [SerializeField] int minAmount = 10;
    [SerializeField] int maxAmount = 10;

    public override void ApplyEffect(GameObject target = null)
    {
        int amount = Random.Range(minAmount, maxAmount + 1);
        PlayerManager.instance.removeCoin(amount);
    }
}
