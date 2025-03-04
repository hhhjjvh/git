using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIStatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        // gameObject.name =statType;
        gameObject.name = GetstatTypeName();
        statName = GetstatTypeName();

        if (statNameText != null)
        {
            statNameText.text = statName;
        }
        statNameText.fontSize = 32;
    }


    // Start is called before the first frame update
    void Start()
    {
        UpdateStatValueUI();
        ui = GetComponentInParent<UI>();

    }

    // Update is called once per frame
    void Update()
    {
        // statName= statType.ToString();
    }
    public string GetstatTypeName()
    {

        switch (statType)
        {
            case StatType.strength: return "¡¶¡ø";
            case StatType.agility: return "√ÙΩ›";
            case StatType.intelligence: return "÷«¡¶";
            case StatType.vitality: return "ÃÂ¡¶";
            case StatType.maxhealth: return "…˙√¸";
            case StatType.armor: return "ŒÔ¿Ì∑¿”˘";
            case StatType.magicresist: return "ƒß∑®øπ–‘";
            case StatType.evasion: return "…¡±‹¬ %";
            case StatType.damage: return "ŒÔ¿Ì…À∫¶";
            case StatType.critChance: return "±©ª˜¬ %";
            case StatType.critPower: return "±©ª˜…À∫¶%";
            case StatType.firDamage: return "ª—Ê…À∫¶";
            case StatType.iceDamage: return "∫Æ±˘…À∫¶";
            case StatType.lightningDamage: return "…¡µÁ…À∫¶";
            default: return "Œ¥÷™";
        }
    }
    public void UpdateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            statValueText.text = GetPlayerStats(playerStats).ToString();
            //statValueText.text = playerStats.GetStat(statType).GetValue().ToString();
            statValueText.color = Color.white;
            statValueText.fontSize = 32;
        }

    }

    private int GetPlayerStats(PlayerStats playerStats)
    {

        int playstat = 0;
        switch (statType)
        {
            case StatType.maxhealth: playstat = playerStats.GetMaxHealth(); break;
            case StatType.damage: playstat = playerStats.GetDamage(); break;
            case StatType.critPower: playstat = playerStats.GetCritPower(); break;
            case StatType.critChance: playstat = playerStats.GetCritChance(); break;
            case StatType.evasion: playstat = playerStats.GetEvasion(); break;
            case StatType.armor: playstat = playerStats.GetArmor(); break;
            case StatType.magicresist: playstat = playerStats.GetMagicResistance(); break;
            case StatType.firDamage: playstat = playerStats.GetFireDamage(); break;
            case StatType.iceDamage: playstat = playerStats.GetIceDamage(); break;
            case StatType.lightningDamage: playstat = playerStats.GetLightningDamage(); break;

            default: playstat = (int)playerStats.GetStat(statType).GetValue(); break;

        }
        return playstat;
    }

    public void changeeStatValueUI(ItemDataEquipment olditem, ItemDataEquipment newitem)
    {

        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            int oldstats = 0;
            int newstats = 0;
            if (olditem != null)
            {
                oldstats = olditem.GetStat(statType);
                oldstats = ChackStats(olditem, oldstats);

            }
            if (newitem != null)
            {
                newstats = newitem.GetStat(statType);
                newstats = ChackStats(newitem, newstats);

            }


            //int oldatatValue = playerStats.GetStat(statType).GetValue();
            int oldatatValue = GetPlayerStats(playerStats);
            int newstatValue = oldatatValue - oldstats + newstats;

            int x = newstatValue - oldatatValue;
            if (x > 0)
            {
                statValueText.color = Color.green;
            }
            else if (x < 0)
            {
                statValueText.color = Color.red;
            }
            else
            {
                statValueText.color = Color.white;
            }

            statValueText.text = oldatatValue.ToString() + " ---> " + newstatValue.ToString() + "    " + x.ToString();
            statValueText.fontSize = 20;
        }


    }

    private int ChackStats(ItemDataEquipment olditem, int oldstats)
    {
        if (statType == StatType.maxhealth) oldstats = olditem.GetStat(StatType.vitality) * 10 + olditem.GetStat(statType);
        if (statType == StatType.damage) oldstats = (int)(olditem.GetStat(StatType.strength) * 3.5f + olditem.GetStat(statType));
        if (statType == StatType.critPower) oldstats =(int)(olditem.GetStat(StatType.strength)*0.1f + olditem.GetStat(statType));
        if (statType == StatType.critChance) oldstats =(int) (olditem.GetStat(StatType.agility) + olditem.GetStat(statType));
        if (statType == StatType.evasion) oldstats =(int) (olditem.GetStat(StatType.agility) + olditem.GetStat(statType));
        if (statType == StatType.armor) oldstats = olditem.GetStat(StatType.vitality) + olditem.GetStat(statType);
        if (statType == StatType.magicresist) oldstats =(int) (olditem.GetStat(StatType.intelligence) * 2.5f + olditem.GetStat(statType));
        if (statType == StatType.firDamage) oldstats = olditem.GetStat(StatType.intelligence) + olditem.GetStat(statType);
        if (statType == StatType.iceDamage) oldstats = olditem.GetStat(StatType.intelligence) + olditem.GetStat(statType);
        if (statType == StatType.lightningDamage) oldstats = olditem.GetStat(StatType.intelligence) + olditem.GetStat(statType);
        return oldstats;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statTooltip.SetDescription(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statTooltip.Hide();

    }
}
