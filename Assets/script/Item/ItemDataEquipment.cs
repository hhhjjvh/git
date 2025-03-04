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
    public int strength;//����
    public int agility;//����
    public int intelligence;//����
    public int vitality;//����

    [Header("Defensive Stats")]
    public int health;//����ֵ
    public int armor;//����
    public int magicresist;//ħ������
    public int evasion;//����

    [Header("Offensive Stats")]
    public int damage;//�˺�
    public int critChance;//������
    public int critPower;//�����˺�

    [Header("Magic Stats")]
    public int firDamage;//�����˺�
    public int iceDamage;//��˪�˺�
    public int lightningDamage;//�����˺�

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

        AddItemDescription(strength, "����");
        AddItemDescription(agility, "����");
        AddItemDescription(intelligence, "����");
        AddItemDescription(vitality, "����");
        AddItemDescription(health, "����ֵ");
        AddItemDescription(armor, "����");
        AddItemDescription(magicresist, "ħ������");
        AddItemDescription(evasion, "����");
        AddItemDescription(damage, "�˺�");
        AddItemDescription(critChance, "������");
        AddItemDescription(critPower, "�����˺�");
        AddItemDescription(firDamage, "�����˺�");
        AddItemDescription(iceDamage, "�����˺�");
        AddItemDescription(lightningDamage, "�����˺�");

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
            case EquipmentType.Weapon: return "����";
            case EquipmentType.Amulet: return "��Ʒ";
            //case EquipmentType.Chest: return "�ؼ�";
            //case EquipmentType.Legs: return "�ȼ�";
           // case EquipmentType.Helmet: return "ͷ��";
           // case EquipmentType.Boots: return "ѥ��";
            case EquipmentType.Armor: return "����";
            case EquipmentType.Flask: return "ҩˮ";
            default: return "δ֪";

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
                //    sb.Append("Ч��" + (i + 1) + ":");
                //}
                //else
                //{
                //    sb.Append("Ч��:");
                //}
                if(itemEffects.Length>1)
                {
                    sb.Append("Ч��" + (i + 1) + ":");
                }
                else
                {
                    sb.Append("Ч��:");
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
            sb.Append("��飺");
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
