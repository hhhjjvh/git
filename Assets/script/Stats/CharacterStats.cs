using System.Collections;
using UnityEngine;


public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    maxhealth,
    armor,
    magicresist,
    evasion,
    damage,
    critChance,
    critPower,
    firDamage,
    iceDamage,
    lightningDamage,


}

public class CharacterStats : MonoBehaviour
{
    [SerializeField] protected int level = 1;
    public EntityFX entityFX;

    [Header("Major Stats")]
    public Stat strength;//Á¦Á¿
    public Stat agility;//Ãô½Ý
    public Stat intelligence;//ÖÇÁ¦
    public Stat vitality;//ÌåÁ¦

    [Header("Defensive Stats")]
    public Stat maxhealth;//×î´óÉúÃüÖµ
    public Stat armor;//»¤¼×
    public Stat magicresist;//Ä§·¨¿¹ÐÔ
    public Stat evasion;//ÉÁ±Ü


    [Header("Offensive Stats")]
    public Stat damage;//ÉËº¦
    public Stat critChance;//±©»÷ÂÊ
    public Stat critPower;//±©»÷ÉËº¦

    [Header("Magic Stats")]
    public Stat firDamage;//»ðÑæÉËº¦
    public Stat iceDamage;//±ùËªÉËº¦
    public Stat lightningDamage;//ÉÁµçÉËº¦

    private int startstrength;
    private int startagility;
    private int startintelligence;
    private int startvitality;
    private int startmaxhealth;
    private int startarmor;
    private int startmagicresist;
    private int startevasion;
    private int startdamage;
    private int startcritChance;
    private int startcritPower;
    private int startfirDamage;
    private int starticeDamage;
    private int startlightningDamage;

    public bool isIgnited; //ÊÇ·ñ±»µãÈ¼
    public bool isChilled; //ÊÇ·ñ±»±ù¶³
    public bool isShocked; //ÊÇ·ñ±»Âé±Ô


    public int health;


    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;
    private float vulnerableTimer;



    private float ignitedDamageCoodlowm = .3f;
    private float ignitedDamageTimer;
    private int ignitedDamage { get; set; }

    private int chilledDamage { get; set; }

    private int shockDamage { get; set; }
    //[SerializeField] private GameObject shockStrike;

    public bool isDead { get; protected set; }
    public bool isInvincible { get; private set; }
    protected bool isVulnerable { get; set; }
    public bool isOverlordBody { get; private set; }

    protected player1 player;

    public System.Action OnHealthChanged;



    public virtual void Awake()
    {
        SetStartStats();
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
       

        health = GetMaxHealth();
        critPower.SetBaseValue(150);

        entityFX = GetComponent<EntityFX>();
        player = PlayerManager.instance.player;
    }

    protected virtual void SetStartStats()
    {
        startstrength = (int)strength.GetValue();
        startagility = (int)agility.GetValue();
        startintelligence = (int)intelligence.GetValue();
        startvitality = (int)vitality.GetValue();
        startmaxhealth = (int)maxhealth.GetValue();
        startarmor = (int)armor.GetValue();
        startmagicresist = (int)magicresist.GetValue();
        startevasion = (int)evasion.GetValue();
        startdamage = (int)damage.GetValue();
        startcritChance = (int)critChance.GetValue();
        startcritPower = (int)critPower.GetValue();
        startfirDamage = (int)firDamage.GetValue();
        starticeDamage = (int)iceDamage.GetValue();
        startlightningDamage = (int)lightningDamage.GetValue();

       
    }
    protected virtual void SetBaseStats()
    {
       // Debug.Log(startmaxhealth);
        strength.SetBaseValue(startstrength);
        agility.SetBaseValue(startagility);
        intelligence.SetBaseValue(startintelligence);
        vitality.SetBaseValue(startvitality);
        maxhealth.SetBaseValue(startmaxhealth);
        armor.SetBaseValue(startarmor);
        magicresist.SetBaseValue(startmagicresist);
        evasion.SetBaseValue(startevasion);
        damage.SetBaseValue(startdamage);
        critChance.SetBaseValue(startcritChance);
        critPower.SetBaseValue(startcritPower);
        firDamage.SetBaseValue(startfirDamage);
        iceDamage.SetBaseValue(starticeDamage);
        lightningDamage.SetBaseValue(startlightningDamage);
        strength.RemoveAllModifiers();
        agility.RemoveAllModifiers();
        intelligence.RemoveAllModifiers();
        vitality.RemoveAllModifiers();
        maxhealth.RemoveAllModifiers();
        armor.RemoveAllModifiers();
        magicresist.RemoveAllModifiers();
        evasion.RemoveAllModifiers();
        damage.RemoveAllModifiers();
        critChance.RemoveAllModifiers();
        critPower.RemoveAllModifiers();
        firDamage.RemoveAllModifiers();
        iceDamage.RemoveAllModifiers();
        lightningDamage.RemoveAllModifiers();

    }
     
    // Update is called once per frame
    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        vulnerableTimer -= Time.deltaTime;


        ignitedDamageTimer -= Time.deltaTime;

        if (OnHealthChanged != null)
        {
            OnHealthChanged();
        }
        if (vulnerableTimer <= 0)
        {
            isVulnerable = false;
        }
        else
        {
            isVulnerable = true;
        }
        //Debug.Log(isVulnerable);
        //Debug.Log(vulnerableTimer);

        if (ignitedTimer <= 0)
        {
            isIgnited = false;
        }
        if (chilledTimer <= 0)
        {
            isChilled = false;
        }
        if (shockedTimer <= 0)
        {
            isShocked = false;
        }
        if (ignitedDamageTimer <= 0 && !isDead)
        {
            if (isIgnited)
            {

                //DecreateHealth(ignitedDamage);
                DoMagicDamage(ignitedDamage);
            }
            if (isChilled)
            {
              
               // DecreateHealth(chilledDamage);
                DoMagicDamage(chilledDamage);
            }
            ChackDead();

            ignitedDamageTimer = ignitedDamageCoodlowm;
        }

    }
    public  int GetLevel()
    {
        return level;

    }
    public virtual void SetLevel(int level)
    {
        this.level = level;
    }
    public void MakeisInvincible(bool isInvincible)
    {
        this.isInvincible = isInvincible;
    }
    public void MakeisInvincible(float duration)
    {
        StartCoroutine(MakeisInvincibles(duration));
    }
    private IEnumerator MakeisInvincibles(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }
    public void MakeVulnerableFor(float duration)
    {
        vulnerableTimer = duration;
        // StartCoroutine(VulnerableCoroutine(duration));
    }
    private IEnumerator VulnerableCoroutine(float duration)
    {
        // Debug.Log("VulnerableCoroutine");
        isVulnerable = true;
        yield return new WaitForSeconds(duration);
        isVulnerable = false;
        // Debug.Log("VulnerableCoroutine  over");
    }
    public void MakeOverlordBody(bool isOverlordBody)
    {
        this.isOverlordBody = isOverlordBody;
    }
    public virtual void IncreaseStatBy(int modifier, int duration, Stat stat)
    {
        StartCoroutine(StatModCoroutine(modifier, duration, stat));

    }
    private IEnumerator StatModCoroutine(int modifier, int duration, Stat stat)
    {
        stat.AddModifier(modifier);
        yield return new WaitForSeconds(duration);
        stat.RemoveModifier(modifier);
    }

    private bool ChackCanAttack(CharacterStats target)
    {
        if (target == null) return false;
        if (target.isInvincible) return false;
        if (target.GetComponent<entity>().isParry && GetComponent<entity>().facingDirection != target.GetComponent<entity>().facingDirection)
        {
            target.entityFX.CreatePopUpText("¸ñµ²", Color.white);
            AudioManager.instance.PlaySFX(2, target.transform);
            return false;
        }
        if (CanAvoidAttack(target)) return false;
        return true;
    }
    public virtual void TakeDamage(int damage)
    {
        
        DecreateHealth(damage);
        ChackDead();
    }

    private void ChackDead()
    {
        if (health <= 0 && !isDead)
        {
            Die();
           
        }
    }

    public virtual void TakeDamage(int damage, Enemy enemy)
    {
       
        //health -= damage;
        DecreateHealth(damage, enemy);
        ChackDead();
    }

    protected virtual void DecreateHealth(int damage)
    {



        health -= damage;
        if (OnHealthChanged != null)
        {
            OnHealthChanged();
        }
    }
    protected virtual void DecreateHealth(int damage, Enemy em)
    {

        health -= damage;
        if (OnHealthChanged != null)
        {
            OnHealthChanged();
        }
    }
    public virtual void IncreaseHealthBy(int amount)
    {
        health += amount;
        entityFX.CreatePopUpText(amount.ToString(), Color.green);
        if (health > GetMaxHealth())
        {
            health = GetMaxHealth();
        }

        if (OnHealthChanged != null)
        {
            OnHealthChanged();
        }
    }


    #region DoDamage
    public virtual void DoDamage(CharacterStats target)
    {
        if (!ChackCanAttack(target)) return;
        int totalDamage;
        DamageChack(target, out totalDamage);
        if (player.ComboCounter == 0)
        {
            AttackSense.instance.HitPause(7);
            player.entityFX.ScreenShake(0.01f, 0.01f);
        }
        else if (player.ComboCounter == 2)
        {
            AttackSense.instance.HitPause(8);
            player.entityFX.ScreenShake(0.05f, 0.04f);

        }
        else if (player.ComboCounter == 2)
        {
            AudioManager.instance.PlaySFX(Random.Range(8, 9), player.transform);
            AttackSense.instance.HitPause(12);
            player.entityFX.ScreenShake(0.2f, 0.3f);
        }

        ItemDataEquipment weapon = Inventory.instance.GetEquipmentType(EquipmentType.Weapon);

        target.TakeDamage(totalDamage);
        if (weapon != null && player.primaryAttackState.isPriimaryAttack)
        {
            weapon.ItemEffect(target.transform);
        }
        // target.DoMagicDamage(target);

    }
    public virtual void DoDamage(CharacterStats target, float idx)
    {
        if (!ChackCanAttack(target)) return;
        int totalDamage;
        DamageChack(target, out totalDamage, idx);

        player.entityFX.ScreenShake(0.1f * idx, 0.1f * idx);

        ItemDataEquipment weapon = Inventory.instance.GetEquipmentType(EquipmentType.Weapon);

        target.TakeDamage(totalDamage);
        if (weapon != null && player.primaryAttackState.isPriimaryAttack)
        {
            weapon.ItemEffect(target.transform);
        }

        // target.DoMagicDamage(target);

    }


    public virtual void DoDamage(CharacterStats target, Enemy enemy)
    {

        if (!ChackCanAttack(target)) return;
        //AttackSense.instance.HitPause(3);
        int totalDamage;
        DamageChack(target, out totalDamage);

        target.TakeDamage(totalDamage, enemy);
       //target.DoMagicDamage(target, enemy);

    }
    public virtual void DoDamage(int damage)
    {

        damage = CheckTargetArmor(this, damage);
        if (damage <= 0)
        {
            entityFX.CreatePopUpText("Î´ÆÆ·À", Color.white);
            return;
        }

        if (isVulnerable)
        {
            damage = (int)(damage * 1.5f);
        }
        this.entityFX.CreatePopUpText(damage.ToString(), Color.white);
        TakeDamage(damage);
    }
    public virtual void DoMaxDamage(float idx)
    {

       int  damage = (int)(GetMaxHealth()* idx);
        if (isVulnerable)
        {
            damage = (int)(damage * 1.5f);
        }
        this.entityFX.CreatePopUpText(damage.ToString(), Color.white);
        TakeDamage(damage);
    }
    private void DamageChack(CharacterStats target, out int totalDamage, float idx = 1)
    {

        //Debug.Log(target.isVulnerable);
        bool canCrit = false;
        bool isbighit = false;
        //target.GetComponent<Enemy>().bigHitState.setBigHit(true);

        totalDamage = GetDamage();
       // Debug.Log(totalDamage);
        totalDamage = CheckTargetArmor(target, totalDamage);
        if (target.isOverlordBody) totalDamage = totalDamage / 2;
        //Debug.Log(totalDamage);
        if (GetComponent<entity>().ComboCounter == 1)
        {
            totalDamage = (int)(totalDamage * 1.2f);
        }
        if (GetComponent<entity>().ComboCounter == 2)
        {
            totalDamage = (int)(totalDamage * 1.4f);
        }
        if (CanCrit())
        {
            totalDamage = CalculateCritDamage(totalDamage);

            if (CanCrit() && Random.Range(0, 100) < 10 * (GetComponent<entity>().ComboCounter + 1))
            {
                if (target.GetComponent<entity>().entityType == entityType.Enemy)
                {
                    target.GetComponent<Enemy>().bigHitState.setBigHit(true);
                }
                else
                {
                    AudioManager.instance.PlaySFX(Random.Range(7, 8), target.transform);
                }
                totalDamage = CalculateCritDamage(totalDamage);
                isbighit = true;
            }

            if (Random.Range(0, 100) < 20 * idx && idx > 1.5f)
            {
                if (target.GetComponent<entity>().entityType == entityType.Enemy)
                {
                    target.GetComponent<Enemy>().bigHitState.setBigHit(true);
                }
                totalDamage = CalculateCritDamage(totalDamage);
                isbighit = true;
            }
            canCrit = true;
        }

        entityFX.CreatHitFX(target.transform, canCrit);


        if (target.isVulnerable)
        {
            totalDamage = (int)(totalDamage * 1.5f);
        }
        float damageRandom = Random.Range(-5f, 5f);
        totalDamage += (int)(totalDamage * (damageRandom / 100f));
        totalDamage = (int)(totalDamage * idx);
        if (totalDamage <= 0)
        {
            totalDamage = 1;
            //target.entityFX.CreatePopUpText("Î´ÆÆ·À", Color.white);
            //return;
        }
        if (canCrit)
        {
            if (isbighit)
            {
                player.entityFX.ScreenShake(0.8f, 0.8f);
                target.entityFX.CreatePopUpText(totalDamage.ToString(), Color.red);
            }
            else
            {
                player.entityFX.ScreenShake(0.1f, 0.1f);
                target.entityFX.CreatePopUpText(totalDamage.ToString(), Color.yellow);
            }
        }
        else
        {
            target.entityFX.CreatePopUpText(totalDamage.ToString(), Color.white);
        }
    }
    #endregion
    #region MagicDamage
    public virtual void DoMagicDamage(CharacterStats target, float idx = 1)
    {

        if (!ChackCanAttack(target)) return;
        int totalDamage;
        MagicDamageChack(target, out totalDamage);

        target.TakeDamage((int)(totalDamage*idx));
    }
    public virtual void DoMagicDamage(CharacterStats target, Enemy enemy)
    {
        if (!ChackCanAttack(target)) return;
        int totalDamage;
        MagicDamageChack(target, out totalDamage);

        target.TakeDamage(totalDamage, enemy);
    }
    public virtual void DoMagicDamage(int damage)
    {
        if (!ChackCanAttack(this)) return;
        damage -= GetMagicResistance();
        if (damage <= 0)
        {
            damage = 1;
        }

        if (isVulnerable)
        {
            damage = (int)(damage * 1.5f);
        }
        if (isIgnited)
        {
            entityFX.CreatePopUpText(ignitedDamage.ToString(), Color.red);
        }
        if (isChilled)
        {
            entityFX.CreatePopUpText(chilledDamage.ToString(), Color.blue);
        }
        if (isShocked)
        {
            this.entityFX.CreatePopUpText(damage.ToString(), Color.yellow);
        }
        //TakeDamage(damage);
        DecreateHealth(damage);
        ChackDead();
    }
    private void MagicDamageChack(CharacterStats target, out int totalDamage)
    {
        //Debug.Log(target);
        int fireDamages = GetFireDamage();
        int iceDamages = GetIceDamage();
        int lightningDamages = GetLightningDamage();

        totalDamage = fireDamages + iceDamages + lightningDamages;
        totalDamage -= target.GetMagicResistance();
        if (target.isOverlordBody) totalDamage = totalDamage / 2;
        if (target.isVulnerable)
        {
            totalDamage = (int)(totalDamage * 1.5f);
        }
        float damageRandom = Random.Range(-3f, 3f);
        totalDamage += (int)(totalDamage * (damageRandom / 100f));
        if (totalDamage <= 0)
        {
            totalDamage = 1;
            //target.entityFX.CreatePopUpText("Î´ÆÆ·À", Color.blue);
            //return;
        }
        //Debug.Log(totalDamage);
        target.entityFX.CreatePopUpText(totalDamage.ToString(), Color.magenta);


        if (Mathf.Max(fireDamages, iceDamages, lightningDamages) < 0) return;

        bool canApplyIgnited = fireDamages >= iceDamages && fireDamages >= lightningDamages;
        bool canApplyChilled = iceDamages >= fireDamages && iceDamages >= lightningDamages;
        bool canApplyShocked = lightningDamages >= fireDamages && lightningDamages >= iceDamages;

        while (!canApplyShocked && !canApplyChilled && !canApplyIgnited)
        {
            if (Random.value < 0.3f && fireDamages > 0)
            {
                canApplyIgnited = true;
                target.ApplyAilments(canApplyIgnited, canApplyChilled, canApplyShocked, this);
                return;
            }
            if (Random.value < 0.5f && iceDamages > 0)
            {
                canApplyChilled = true;
                target.ApplyAilments(canApplyIgnited, canApplyChilled, canApplyShocked, this);
                return;
            }
            if (Random.value < 1f && lightningDamages > 0)
            {
                canApplyShocked = true;
                target.ApplyAilments(canApplyIgnited, canApplyChilled, canApplyShocked, this);
                return;
            }
        }
        if (canApplyIgnited)
        {
            int fireDamage = (int)(fireDamages * .15f);
            if (fireDamage <= 0) fireDamage = 1;

            target.SetupIgniteDamage(fireDamage);
        }
        if (canApplyChilled)
        {
            int iceDamage = (int)(iceDamages * .1f);
            if (iceDamage <= 0) iceDamage = 1;


            target.SetupChillDamage(iceDamage);
        }
        if (canApplyShocked)
        {
            int lightningDamage = (int)(lightningDamages * .3f);
            if (lightningDamage <= 0) lightningDamage = 1;
            target.SetupShockDamage(lightningDamage);
        }

        target.ApplyAilments(canApplyIgnited, canApplyChilled, canApplyShocked,this);
    }


    public void ApplyAilments(bool isIgnited, bool isChilled, bool isShocked, CharacterStats myself)
    {
        //if (this.isIgnited&&isChilled&&isShocked) return;
        // if(this.isIgnited||this.isChilled||this.isShocked) return;
        //Debug.Log("Apply Ailments");

        bool canApplyIgnited = !this.isIgnited && !this.isChilled && !this.isShocked && isIgnited;
        bool canApplyChilled = !this.isIgnited && !this.isChilled && !this.isShocked && isChilled;
        bool canApplyShocked = !this.isIgnited && !this.isChilled && !this.isShocked && isShocked;

        if (canApplyIgnited)
        {
            this.isIgnited = isIgnited;
            ignitedTimer = 5;
            entityFX.IgniteFxFor(5);
            //Debug.Log("Ignited");
        }
        if (canApplyChilled)
        {
            this.isChilled = isChilled;
            chilledTimer = 4;
            entityFX.ChillFxFor(4);
            GetComponent<entity>().SlowEnityBy(.7f, 4);
            //Debug.Log("Chilled");
        }
        if (isShocked)
        {
            if (!this.isShocked)
            {


                this.isShocked = isShocked;
                shockedTimer = 2;
                entityFX.ShockFxFor(2);
            }


            //if (GetComponent<player1>() != null) return;

            HitShock(myself);



            // Debug.Log("Shocked");
        }

        // this.isIgnited = isIgnited;
        //this.isChilled = isChilled;
        // this.isShocked = isShocked;
    }


   

    public void SetupIgniteDamage(int damage) => ignitedDamage = damage;
    public void SetupChillDamage(int damage) => chilledDamage = damage;
    public void SetupShockDamage(int damage) => shockDamage = damage;
    #endregion

    protected int CheckTargetArmor(CharacterStats target, int totalDamage)
    {
        if (target.isChilled)
        {
            totalDamage -= (int)(target.GetArmor() * 0.7f);
        }
        else
        {
            totalDamage -= target.GetArmor();
        }

        //totalDamage -= target.armor.GetValue();
        if (totalDamage < 0) totalDamage = 0;
        return totalDamage;
    }
    protected bool CanAvoidAttack(CharacterStats target)
    {
        int totalEvasion = target.GetEvasion();
        if (isShocked) totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {

            target.OnEvasion(target);
            return true;
        }
        return false;
    }

    public virtual void OnEvasion(CharacterStats target)
    {
        target.entityFX.CreatePopUpText("ÉÁ±Ü", Color.white);
    }
    private void HitShock(CharacterStats myself)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 50f);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer(myself.GetComponent<entity>().GetAttackLayerName()) && !hit.GetComponent<CharacterStats>().isDead)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
                
            }
        }
        float closestDistances = Mathf.Infinity;
        foreach (var hit in colliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer(myself.GetComponent<entity>().GetAttackLayerName()) && Vector2.Distance(transform.position, hit.transform.position) > 2f 
                && !hit.GetComponent<CharacterStats>().isDead)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistances)
                {
                    closestDistances = distance;
                    closestEnemy = hit.transform;
                }

            }

        }

        if (closestEnemy != null)
        {
            // Debug.Log("Shocked");
            //GameObject newshock = Instantiate(shockStrike, transform.position, Quaternion.identity);
            GameObject newshock = PoolMgr.Instance.GetObj("Thunders", transform.position, Quaternion.identity);
           
            newshock.GetComponent<ThunderStrikeContorller>().Setup(closestEnemy.GetComponent<CharacterStats>(), shockDamage);
        }
    }



    protected virtual void Die()
    {

        isDead = true;
       // Destroy(gameObject, 10f);
        //throw new NotImplementedException();

    }
    protected bool CanCrit()
    {
        int totalCritChance = GetCritChance();
        if (Random.Range(0, 100) < totalCritChance)
        {
            return true;
        }
        return false;
    }
    protected int CalculateCritDamage(int damage)
    {
        float critMultiplier = (float)(GetCritPower()) / 100f;
        float totalDamage = damage * critMultiplier;
        return Mathf.RoundToInt(totalDamage);
    }

    #region Get Final Stat Value

    public int GetMaxHealth()
    {
        return (int)(maxhealth.GetValue() + vitality.GetValue() * 30);
    }


    public int GetDamage()
    {
        return (int)(damage.GetValue() + strength.GetValue()*2.5f);
       

    }

    public int GetCritPower()
    {
        return (int)(critPower.GetValue() + strength.GetValue()*0.1f);
    }

    public int GetCritChance()
    {
        return (int)(critChance.GetValue() + agility.GetValue());
    }

    public int GetEvasion()
    {
        return (int)(evasion.GetValue() + agility.GetValue());
    }
    public int GetArmor()
    {
        return (int)(armor.GetValue()); //+ vitality.GetValue());
        //return armor.GetValue() + vitality.GetValue()*2;

    }

    public int GetMagicResistance()
    {
        return (int)(magicresist.GetValue());// intelligence.GetValue() * 2.5f);

       // return magicresist.GetValue() + intelligence.GetValue() * 3;
    }

    public int GetFireDamage()
    {
        return (int)(firDamage.GetValue() + intelligence.GetValue());
       // return firDamage.GetValue() + intelligence.GetValue();

    }

    public int GetIceDamage()
    {
        return (int)(iceDamage.GetValue() + intelligence.GetValue());
        //return iceDamage.GetValue() + intelligence.GetValue();
    }

    public int GetLightningDamage()
    {
        return (int)(lightningDamage.GetValue() + intelligence.GetValue());
        //return lightningDamage.GetValue() + intelligence.GetValue();
    }
    #endregion
   
    public Stat GetStat(StatType buffType)
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
                return maxhealth;
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
                return null;

        }
    }
}
