using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
  [Header ("Major Stats")]
    public Stat strength;//����
    public Stat agility;//����
    public Stat intelligence;//����
    public Stat vitality;//����

    [Header ("Defensive Stats")]
    public Stat maxhealth;//�������ֵ
    public Stat armor;//����
    public Stat magicresist;//ħ������
    public Stat evasion;//����

    [Header ("Offensive Stats")]
    public Stat damage;//�˺�
    public Stat critChance;//������
    public Stat critPower;//�����˺�

    [Header ("Magic Stats")]
    public Stat firDamage;//�����˺�
    public Stat iceDamage;//��˪�˺�
    public Stat lightningDamage;//�����˺�
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
   
