using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldoun;
    public int needMana;
    public float cooldownTimer { get; protected set; }

    protected player1 player=> PlayerManager.instance.player;
    protected virtual void Awake()
    {
        //Debug.Log(player);
        

    }
    protected virtual void Start()
    {
        //Debug.Log(player);
        //player = PlayerManager.instance.player;
        CheckUnlock();
    }
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }
    public virtual bool CanUseSkill()
    {
       
        if (cooldownTimer <= 0)
        {
            if (player.GetComponent<PlayerStats>().mana < needMana)
            {
                player.entityFX.CreatePopUpText("魔力不足", Color.red);
                AudioManager.instance.PlaySFX(17, null);
                return false;
            }
            player.GetComponent<PlayerStats>().mana -= needMana;

            UseSkill();
            cooldownTimer = cooldoun;
            return true;
           
        }
        //Debug.Log("技能冷却中");
        
        return false;
    }
    public virtual void UseSkill()
    {

    }
    public virtual void CheckUnlock()
    {

    }
    protected virtual Transform closeEnemy(Transform checktransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checktransform.position, 50f);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead)
            {
                float distance = Vector2.Distance(checktransform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }
        if (closestEnemy == null)
        {
            closestEnemy = checktransform;
        }
        return closestEnemy;
    }
}
