using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IceAndFireEffect", menuName = "Data/Item effect/IceAndFireEffect")]
public class IceAndFireEffect : ItemEffect
{
    [SerializeField] private GameObject iceAndFireEffectPrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform enemyPosition)
    {
        //base.ExecuteEffect(enemyPosition);
        player1 player =PlayerManager.instance.player;
        bool isAttack = player.ComboCounter == 2;

        if (isAttack)
        {
            GameObject iceAndFireEffect = Instantiate(iceAndFireEffectPrefab, enemyPosition.position, player.transform.rotation);
            iceAndFireEffect.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity*player.facingDirection, 0);
            Destroy(iceAndFireEffect, 1f);
        }
       

    }


}
