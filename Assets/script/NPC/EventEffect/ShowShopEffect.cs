using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Event/Effects/ShowShopEffect")]
public class ShowShopEffect : EventEffect
{
    
    public override void ApplyEffect(GameObject target = null)
    {
        UI.instance.SwithToShop();
    }
}