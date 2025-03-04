using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using static System.Net.Mime.MediaTypeNames;


public class UIInGame : MonoBehaviour
{
    [SerializeField] private Slider HealthSlider;
    [SerializeField] private Slider ManaSlider;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private PlayerStats myStats;

    [SerializeField] private Image dashimage;
    [SerializeField] private Image parryimage;
    [SerializeField] private Image crystalimage;
    [SerializeField] private Image swordimage;
    [SerializeField] private Image blackholeimage;
    [SerializeField] private Image flaskimage;

    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private TextMeshProUGUI CoinCount;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float coinAmount;
    [SerializeField] private float increasreRate;

    [SerializeField] private TextMeshProUGUI healtext;
    [SerializeField] private TextMeshProUGUI muanatext;
    [SerializeField] private TextMeshProUGUI xptext;
    [SerializeField] private TextMeshProUGUI lvtext;
    [SerializeField] private TextMeshProUGUI Timetext;



    [SerializeField] private TextMeshProUGUI flaskcount;
    //[SerializeField] private float dashcooldown;

    private SkillManager skill;
    
    // Start is called before the first frame update
    void Start()
    {
        if (myStats != null)
        {
            myStats.OnHealthChanged += UpdateSliderUI;
        }
        skill = SkillManager.instance;
        soulsAmount = PlayerManager.instance.GetCurrency();
        coinAmount = PlayerManager.instance.GetCoin();
    }

    // Update is called once per frame
    void Update()
    {
        Timetext.text = TimeManager.Instance.GetFormattedPlaytime();
        healtext.text = myStats.health.ToString() + "/" + myStats.GetMaxHealth().ToString();
        muanatext.text = myStats.mana.ToString() + "/" + myStats.GetMaxMana().ToString();
        xptext.text = myStats.GetExperience().ToString() + "/" + myStats.GetNeedExperience().ToString();
        lvtext.text ="Level:"+ myStats.GetLevel().ToString();
        flaskcount.text = Inventory.instance.GetFlaskSize().ToString();
        UpdataSoulsUI();
        UpdateCoinUI();
        if (Input.GetKeyDown(KeyCode.L) && skill.dash.dashUnlocked)
        {
            SetCooldownOf(dashimage);
        }
        if (Input.GetKeyDown(KeyCode.U) && skill.parry.parryUnlocked)
        {
            SetCooldownOf(parryimage);
        }
        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked )
        {
            SetCooldownOf(crystalimage);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && skill.sword.swordUnlocked)
        {
            SetCooldownOf(swordimage);
        }
        if (Input.GetKeyDown(KeyCode.R) && skill.blackhole.blackholeUnlocked && skill.blackhole.cooldownTimer <= 0)
        {
            SetCooldownOf(blackholeimage);
        }
        ItemDataEquipment flask = Inventory.instance.GetEquipmentType(EquipmentType.Flask);
        if (Input.GetKeyDown(KeyCode.Alpha1) && flask != null)
        {
            SetCooldownOf(flaskimage);
        }


        CheckCooldownOf(dashimage, skill.dash.cooldoun);
        CheckCooldownOf(parryimage, skill.parry.cooldoun);
        CheckCooldownOf(crystalimage, skill.crystal.cooldoun);
        CheckCooldownOf(swordimage, skill.sword.cooldoun);
        CheckCooldownOf(blackholeimage, skill.blackhole.cooldoun);
        CheckCooldownOf(flaskimage, Inventory.instance.flaskCooldown);

    }

    private void UpdataSoulsUI()
    {
        if (soulsAmount < PlayerManager.instance.GetCurrency())
        {
            soulsAmount += increasreRate * Time.deltaTime;
        }
        else
        {
           
            soulsAmount = PlayerManager.instance.GetCurrency();
        }

       
        

        currentSouls.text = "Áé»ê£º" + ((int)soulsAmount).ToString("#,#");
    }
    private void UpdateCoinUI()
    {
        if (coinAmount < PlayerManager.instance.GetCoin())
        {
            coinAmount += increasreRate/10 * Time.deltaTime;
        }
        else
        {
            coinAmount = PlayerManager.instance.GetCoin();
        }
        CoinCount.text = ((int)coinAmount).ToString("#,#");
    }

    private void UpdateSliderUI()
    {
        HealthSlider.maxValue = myStats.GetMaxHealth();
        HealthSlider.value = myStats.health;
        ManaSlider.value = PlayerManager.instance.player.GetComponent<PlayerStats>().mana;
        ManaSlider.maxValue = PlayerManager.instance.player.GetComponent<PlayerStats>().GetMaxMana();
        xpSlider.value = PlayerManager.instance.player.GetComponent<PlayerStats>().GetExperience();
        xpSlider.maxValue = PlayerManager.instance.player.GetComponent<PlayerStats>().GetNeedExperience();
    
    }
    private void SetCooldownOf(Image image)
    {
        if(image.fillAmount<=0)
        {
            image.fillAmount = 1;
        }


    }
    private void CheckCooldownOf(Image image, float cooldown)
    {
        if (image.fillAmount > 0)
        {
            image.fillAmount -= Time.deltaTime / cooldown;
        }
    }
}
