using UnityEngine;

public class PlayerStats : CharacterStats, ISaveManager
{
   // private player1 player;
    public Stat maxmana;//最大魔法值
    public int mana;//当前魔法值
    private float manaRegen;//魔法回复

    //private Enemy enemy;
    [Header("Level detalls")]
    //[SerializeField] private int level = 1;
    private int experience = 0;
    private float canLevelbase = 15;
    private int needExperience = 10;

    private float Timer;
    //private bool isSetHealth = false;

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = .1f;
    
   
    protected override void Start()
    {
        base.Start();
        ModifyLevelbase();
        ApplyLevelModifiers();
        
        Invoke("StartBar", 0.2f);

        Timer = 0.2f;

    }

    protected override void Update()
    {
        base.Update();
       

        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
       
        ManaRegen();

    }

    private void StartBar()
    {
        mana = GetMaxMana();
        health = GetMaxHealth();
    }

    public int GetMaxMana()
    {
        return (int)(maxmana.GetValue() + intelligence.GetValue() * 8);


    }
    public void addMana(int mana)
    {
        this.mana += mana;
       
        entityFX.CreatePopUpText(mana.ToString(), new Color(14f / 255f, 160f / 255f, 255f / 255f));
        if (this.mana > GetMaxMana())
        {
            this.mana = GetMaxMana();
        }
    }
    public void ManaRegen()
    {
        if (Timer <= 0&& mana < GetMaxMana())
        {
            manaRegen=800/(maxmana.GetValue() + intelligence.GetValue() * 8);
            mana++;
            Timer = manaRegen;
        }

    }
   
    public void AddExperience(int exp)
    {
        experience += exp;
        ToLevel();
    }
    public int GetExperience()
    {
        return experience;
    }
    public int GetNeedExperience()
    {
        return needExperience;
    }



    public void ToLevel()
    {
        if (experience >= needExperience)
        {
            level++;
            experience -= needExperience;
            canLevelbase += (float)(canLevelbase * percentageModifier);
            needExperience =(int) (canLevelbase * level * level);
            AddLevelModifiers();
            health = GetMaxHealth();
            mana = GetMaxMana();

            player.entityFX.CreatePopUpText("等级提升", Color.green);
            PoolMgr.Instance.GetObj("Lv_op", transform.position, Quaternion.identity);
            AudioManager.instance.PlaySFX(35, null);
        }

    }

    private void AddLevelModifiers()
    {
        SetBaseStats();
        ApplyLevelModifiers();
        Inventory.instance.AddEquipmentModifiers();
       
    }

    private void Modify(Stat stat)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = stat.GetValue() * percentageModifier*Random.Range(0.95f, 1.05f);
            stat.AddModifier(modifier);
        }
    }
    private void ModifyLevelbase()
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = canLevelbase * percentageModifier;
            //Debug.Log(modifier);
            canLevelbase += modifier;
        }
    }
    private void addModify(Stat stat)
    {
        float modifier = stat.GetValue() * percentageModifier*Random.Range(0.95f, 1.05f);
        stat.AddModifier(modifier);
    }
    private void ApplyLevelModifiers()
    {


        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        /*Modify(magicresist);
        Modify(armor);
        Modify(maxhealth);
        Modify(evasion);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(firDamage);
        Modify(iceDamage);
        Modify(lightningDamage);
        */

    }
    public override void TakeDamage(int damage, Enemy enemy)
    {
        base.TakeDamage(damage, enemy);
        player.MakeDamageFX(damage, enemy);
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        player.MakeDamageFX(damage,null);
    }
    protected override void Die()
    {
        base.Die();
        player.Die();
        GetComponent<PlayerItemDrop>()?.GenerateDropItems();
    }

   


    protected override void DecreateHealth(int damage)
    {
        base.DecreateHealth(damage);
        ItemDataEquipment item = Inventory.instance.GetEquipmentType(EquipmentType.Armor);
        if (item != null)
        {
            item.ItemEffect(player.transform);
        }

    }
    protected override void DecreateHealth(int damage, Enemy em)
    {
        base.DecreateHealth(damage, em);
        ItemDataEquipment item = Inventory.instance.GetEquipmentType(EquipmentType.Armor);
        if (item != null)
        {
            item.ItemEffect(em.transform);
        }
    }
    public override void OnEvasion(CharacterStats target)
    {
        base.OnEvasion(target);
        player.skillManager.dodge.CreateMirageOnDodge();
    }
    public virtual void CloneDoDamage(CharacterStats target, float multiplier)
    {
        bool canCrit = false;
        if (target.isInvincible || CanAvoidAttack(target)) return;

        int totalDamage = GetDamage();
        if (multiplier > 0) totalDamage = (int)(totalDamage * multiplier);

        if (CanCrit())
        {
            totalDamage = CalculateCritDamage(totalDamage);
            canCrit = true;
        }
        entityFX.CreatHitFX(target.transform, canCrit);
        if (isVulnerable)
        {
            totalDamage = (int)(totalDamage * 1.5f);
        }
        totalDamage = CheckTargetArmor(target, totalDamage);
        // totalDamage = (int)(0.5f * totalDamage);
        float damageRandom = Random.Range(-3f, 3f);
        totalDamage += (int)(totalDamage * (damageRandom / 100f));
        if (canCrit)
        {
            target.entityFX.CreatePopUpText(totalDamage.ToString(), Color.yellow);
        }
        else
        {
            target.entityFX.CreatePopUpText(totalDamage.ToString(), Color.white);
        }

        target.TakeDamage(totalDamage);

    }

    public void LoadData(GameData data)
    {
        this.level = data.level;
        this.experience = data.experience;
        this.needExperience = data.needExperience;
       // AddLevelModifiers();
    }

    public void SaveData(ref GameData data)
    {
        data.level = this.level;
        data.experience = this.experience;
        data.needExperience = this.needExperience;
        data.position= transform.position;
    }
}
