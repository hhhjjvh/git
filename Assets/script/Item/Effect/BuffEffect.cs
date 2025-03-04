using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  [Header ("Major Stats")]
    public Stat strength;//Á¦Á¿
    public Stat agility;//Ãô½Ý
    public Stat intelligence;//ÖÇÁ¦
    public Stat vitality;//ÌåÁ¦

    [Header ("Defensive Stats")]
    public Stat maxhealth;//×î´óÉúÃüÖµ
    public Stat armor;//»¤¼×
    public Stat magicresist;//Ä§·¨¿¹ÐÔ
    public Stat evasion;//ÉÁ±Ü

    [Header ("Offensive Stats")]
    public Stat damage;//ÉËº¦
    public Stat critChance;//±©»÷ÂÊ
    public Stat critPower;//±©»÷ÉËº¦

    [Header ("Magic Stats")]
    public Stat firDamage;//»ðÑæÉËº¦
    public Stat iceDamage;//±ùËªÉËº¦
    public Stat lightningDamage;//ÉÁµçÉËº¦
*/



[CreateAssetMenu(fileName = "BuffEffect", menuName = "Data/Item effect/BuffEffect")]
public class BuffEffect : ItemEffect
{
    [SerializeField] private int buffAmount;
    [SerializeField] private int buffDuration;
    [SerializeField] private StatType buffType;
    private PlayerStats playerStats;

    public override void ExecuteEffect(Transform enemyPosition)
    {
        //base.ExecuteEffect(enemyPosition);
       // AudioManager.instance.PlaySFX(23, null);
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.IncreaseStatBy(buffAmount, buffDuration,playerStats.GetStat(buffType));

       
    }
   /* private Stat StatToModify()
    {
        switch (buffType)
        {
            case BuffType.strength:
                return playerStats.strength;
            case BuffType.agility:
                return playerStats.agility;
            case BuffType.intelligence:
                return playerStats.intelligence;
            case BuffType.vitality:
                return playerStats.vitality;
            case BuffType.maxhealth:
                return playerStats.maxhealth;
            case BuffType.armor:
                return playerStats.armor;
            case BuffType.magicresist:
                return playerStats.magicresist;
            case BuffType.evasion:
                return playerStats.evasion;
            case BuffType.damage:
                return playerStats.damage;
            case BuffType.critChance:
                return playerStats.critChance;
            case BuffType.critPower:
                return playerStats.critPower;
            case BuffType.firDamage:
                return playerStats.firDamage;
            case BuffType.iceDamage:
                return playerStats.iceDamage;
            case BuffType.lightningDamage:
                return playerStats.lightningDamage;
            default:
                return null;

        }
    }
    */
}
   
