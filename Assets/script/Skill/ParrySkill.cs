using UnityEngine;
using UnityEngine.UI;

public class ParrySkill : Skill
{
    public bool parryUnlocked { get; private set; }
    public bool restoreUnlocked { get; private set; }
    public bool parryWithMirageUnlocked { get; private set; }
    [SerializeField] private UISkillTreeSlot parryUnlockedButton;
    [SerializeField] private UISkillTreeSlot restoreUnlockedButton;
    [SerializeField] private UISkillTreeSlot parryWithMirageUnlockedButton;

    [Range(0f, 1f)]
    [SerializeField] private float restoreHealthPerentage;
    public bool restorUnlocked;


    public override void UseSkill()
    {
        base.UseSkill();
        if (cooldownTimer <= 0)
        {
            if (restoreUnlocked)
            {
                int restoreHealth = (int)(player.stats.GetMaxHealth() * restoreHealthPerentage);
                player.stats.IncreaseHealthBy(restoreHealth);
            }

        cooldownTimer = cooldoun;
        }
    }
    protected override void Start()
    {
        base.Start();
        parryUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockRestore);
        parryWithMirageUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockParryWithMirage);
    }
    public override void CheckUnlock()
    {
        UnlockParry();
        UnlockRestore();
        UnlockParryWithMirage();

    }
    public void UnlockParry()
    {
        if (parryUnlockedButton.unlocked)
        {
            parryUnlocked = true;
        }
        else
        {
            parryUnlocked = false;
        }
    }
    public void UnlockRestore()
    {
        if (restoreUnlockedButton.unlocked)
        {
            restoreUnlocked = true;
        }
        else
        {
            restoreUnlocked = false;
        }

    }
    public void UnlockParryWithMirage()
    {
        if (parryWithMirageUnlockedButton.unlocked)
        {
            parryWithMirageUnlocked = true;
        }
        else
        {
            parryWithMirageUnlocked = false;
        }
    }
    public void MakeMirageOnParry(Transform target)
    {
        if (parryWithMirageUnlocked)
        {
            SkillManager.instance.clone.CreateCloneOnCounterAttack(target);
        }
    }
}
