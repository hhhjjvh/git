using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myitemDrop;
    public Stat soulsDropAmount;
    public Stat experienceDropAmount;
    public float soulsDropAmounts;
    public float experienceDropAmounts;

    [Header("Level detalls")]


    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = .1f;

    protected override void SetStartStats()
    {
        base.SetStartStats();
        soulsDropAmounts= soulsDropAmount.GetValue();
        experienceDropAmounts = experienceDropAmount.GetValue();
    }
    protected override void SetBaseStats()
    {
        base.SetBaseStats();
        soulsDropAmount.SetBaseValue(soulsDropAmounts);
        experienceDropAmount.SetBaseValue(experienceDropAmounts);
        soulsDropAmount.RemoveAllModifiers();
        experienceDropAmount.RemoveAllModifiers();
    }

    private void Modify(Stat stat)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = stat.GetValue() * percentageModifier;
            //Debug.Log(modifier);
            stat.AddModifier(modifier);
        }
    }
    private void RemoveModify(Stat stat)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = stat.GetValue() * percentageModifier;
            Debug.Log(modifier);
            stat.RemoveModifier(modifier);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (enemy == null) return;
        enemy.MakeDamageFX(damage);
        //Debug.Log("Enemy took damage");
    }

    protected override void Die()
    {
        base.Die();
        if (enemy == null) return;

        enemy.Die();

        EnemyDrops();
    }

    private void EnemyDrops()
    {
        PlayerManager.instance.currency += (int)soulsDropAmount.GetValue();
        PlayerManager.instance.player.GetComponent<PlayerStats>().AddExperience((int)experienceDropAmount.GetValue());

        myitemDrop.GenerateDropItems();
        float max1 = soulsDropAmounts / 10f; if (max1 < 1) max1 = 1; if (max1 > 2) max1 = 2;
        int count1 = (int)Random.Range(0, max1);
        for (int i = 0; i < count1; i++)
        {
            GameObject Copper = PoolMgr.Instance.GetObj("EnergyBall", new Vector2(Random.Range(-2f, 2f)+transform.position.x,
                Random.Range(0, 2)+transform.position.y), Quaternion.identity);
            Copper.transform.SetParent(transform.parent);
            // ItemFactory.Instance.GetItem(ItemType.EnergyBall, (Vector2)ballPoint.transform.position + Random.insideUnitCircle).AddToController();
        }
        float max = soulsDropAmounts / 5f; if (max < 1) max = 1; if (max > 10) max = 10;
        int count2 =(int) Random.Range(0, max);
        for (int i = 0; i < count2; i++)
        {   //ItemFactory.Instance.GetCoin(CoinType.Coppers, (Vector2)ballPoint.transform.position + Random.insideUnitCircle * 2).AddToController();
            GameObject Copper = PoolMgr.Instance.GetObj("Coppers", new Vector2(Random.Range(-2f, 2f) + transform.position.x,
                Random.Range(0, 2) + transform.position.y), Quaternion.identity);
            Copper.GetComponentInChildren<CoinTrigger>().SetCoin((int)(Random.Range(1f, 3f)*(1+ level*0.15f)));
            Copper.transform.SetParent(transform.parent);

        }
    }

    protected override void Start()
    {
        //soulsDropAmount.SetBaseValue(100);
        ApplyLevelModifiers();
        base.Start();
        enemy = GetComponent<Enemy>();

        myitemDrop = GetComponent<ItemDrop>();
    }
    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        isDead= false;
        AddLevelModifiers();
    }
    private void AddLevelModifiers()
    {

        SetBaseStats();
        ApplyLevelModifiers();
        health = GetMaxHealth();
        //Debug.Log(health);


    }
    private void ApplyLevelModifiers()
    {


        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        /*
               Modify(armor);
               Modify(magicresist);
               Modify(maxhealth);
               Modify(evasion);

               Modify(damage);
               Modify(critChance);
               Modify(critPower);

               Modify(firDamage);
               Modify(iceDamage);
               Modify(lightningDamage);
        */

        Modify(soulsDropAmount);
        Modify(experienceDropAmount);
    }
    private void RemoveLevelModifiers()
    {
        RemoveModify(strength);
        RemoveModify(agility);
        RemoveModify(intelligence);
        RemoveModify(vitality);


        /*
          RemoveModify(armor);
        RemoveModify(magicresist);
              RemoveModify(maxhealth);
              RemoveModify(evasion);

              RemoveModify(damage);
              RemoveModify(critChance);
              RemoveModify(critPower);

              RemoveModify(firDamage);
              RemoveModify(iceDamage);
              RemoveModify(lightningDamage);
       */

        RemoveModify(soulsDropAmount);
        RemoveModify(experienceDropAmount);
    }

    protected override void Update()
    {
        base.Update();
    }

}
