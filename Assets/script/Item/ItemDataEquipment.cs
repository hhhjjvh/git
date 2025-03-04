using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public enum EquipmentType
{
    Weapon, Armor, Amulet,Flask
}

[CreateAssetMenu(fileName = "Equipment", menuName = "Data/Equipment")]
public class ItemDataEquipment : ItemData
{
    public ItemEffect[] itemEffects;

    public float itemCooldown;

    public EquipmentType equipmentType;

    [TextArea]
    public string itemEffectDescription;


    [Header("Major Stats")]
    public int strength;//力量
    public int agility;//敏捷
    public int intelligence;//智力
    public int vitality;//体力

    [Header("Defensive Stats")]
    public int health;//生命值
    public int armor;//护甲
    public int magicresist;//魔法抗性
    public int evasion;//闪避

    [Header("Offensive Stats")]
    public int damage;//伤害
    public int critChance;//暴击率
    public int critPower;//暴击伤害

    [Header("Magic Stats")]
    public int firDamage;//火焰伤害
    public int iceDamage;//冰霜伤害
    public int lightningDamage;//闪电伤害

    [Header("Craft requirements")]
    public List<InventoryItem> craftRequirements;

    private int minDescriptionLength;

    public void ItemEffect(Transform enemyPosition)
    {
        if(itemEffects == null) { return; }
        foreach (ItemEffect itemEffect in itemEffects)
        {
            itemEffect.ExecuteEffect(enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats =PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.maxhealth.AddModifier(health);
        playerStats.armor.AddModifier(armor);
        playerStats.magicresist.AddModifier(magicresist);
        playerStats.evasion.AddModifier(evasion);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.firDamage.AddModifier(firDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);

    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);
            
        playerStats.maxhealth.RemoveModifier(health);
        playerStats.armor.RemoveModifier(armor);
        playerStats.magicresist.RemoveModifier(magicresist);
        playerStats.evasion.RemoveModifier(evasion);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.firDamage.RemoveModifier(firDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
    }

    public override string GetDescription()
    {
        sb.Length = 0;
        minDescriptionLength = 0;

        AddItemDescription(strength, "力量");
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智力");
        AddItemDescription(vitality, "体力");
        AddItemDescription(health, "生命值");
        AddItemDescription(armor, "护甲");
        AddItemDescription(magicresist, "魔法抗性");
        AddItemDescription(evasion, "闪避");
        AddItemDescription(damage, "伤害");
        AddItemDescription(critChance, "暴击率");
        AddItemDescription(critPower, "暴击伤害");
        AddItemDescription(firDamage, "火焰伤害");
        AddItemDescription(iceDamage, "寒冰伤害");
        AddItemDescription(lightningDamage, "闪电伤害");

        if (minDescriptionLength < 0)
        {
            for (int i = 0; i < 0 - minDescriptionLength; i++)
            {
                sb.AppendLine();
                sb.Append(" ");
            }
        }



        return sb.ToString();
    }

    public string GetEquipmentTypeName()
    {
       
        switch (equipmentType)
        {
            case EquipmentType.Weapon: return "武器";
            case EquipmentType.Amulet: return "饰品";
            //case EquipmentType.Chest: return "胸甲";
            //case EquipmentType.Legs: return "腿甲";
           // case EquipmentType.Helmet: return "头盔";
           // case EquipmentType.Boots: return "靴子";
            case EquipmentType.Armor: return "防具";
            case EquipmentType.Flask: return "药水";
            default: return "未知";

        }

    }
    public int GetStat(StatType buffType)
    {
        switch (buffType)
        {
            case StatType.strength:
                return strength;
            case StatType.agility:
                return agility;
            case StatType.intelligence:
                return intelligence;
            case StatType.vitality:
                return vitality;
            case StatType.maxhealth:
                return health;
            case StatType.armor:
                return armor;
            case StatType.magicresist:
                return magicresist;
            case StatType.evasion:
                return evasion;
            case StatType.damage:
                return damage;
            case StatType.critChance:
                return critChance;
            case StatType.critPower:
                return critPower;
            case StatType.firDamage:
                return firDamage;
            case StatType.iceDamage:
                return iceDamage;
            case StatType.lightningDamage:
                return lightningDamage;
            default:
                return 0;

        }
    }
    public string GetEffectName()
    {
        sb.Length = 0;
        minDescriptionLength = 0;
        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i].itemEffectDescription.Length > 0)
            {
                sb.AppendLine();
                //if (i > 0)
                //{
                //    sb.Append("效果" + (i + 1) + ":");
                //}
                //else
                //{
                //    sb.Append("效果:");
                //}
                if(itemEffects.Length>1)
                {
                    sb.Append("效果" + (i + 1) + ":");
                }
                else
                {
                    sb.Append("效果:");
                }
                // sb.AppendLine();
                sb.Append(itemEffects[i].itemEffectDescription);
                minDescriptionLength++;
            }
        }
        return sb.ToString();
    }
    public string GetItemName()
    {
        sb.Length = 0;
        minDescriptionLength = 0;
        if (itemEffectDescription!=null&&itemEffectDescription.Length > 0)
        {
            sb.AppendLine();
            sb.Append("简介：");
            // sb.AppendLine();
            sb.Append(itemEffectDescription);
            minDescriptionLength++;
        }
        return sb.ToString();

    }
    private void AddItemDescription(int value,string name)
    {
        if (value != 0)
        {
            if(sb.Length>0)
            {
                sb.AppendLine();
            }

            if (value > 0)
            {
                sb.Append(name +": " + value);
            }

            minDescriptionLength++;
        }
    }
}
