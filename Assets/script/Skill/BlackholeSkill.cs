using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackholeSkill : Skill
{
    [SerializeField] private UISkillTreeSlot blackholeUnlockButton;
    public bool blackholeUnlocked { get;private set; }


    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneAttackCooldown;
    [SerializeField] private float blackholeDuration;

    BlackhoieController blackholeController;
    public override bool CanUseSkill()
    {
       
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        GameObject blackhole = Instantiate(blackholePrefab, player.transform.position, Quaternion.identity);
        blackholeController = blackhole.GetComponent<BlackhoieController>();
        blackholeController.SetupBlackhole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneAttackCooldown, blackholeDuration);
    }

    private void UnclockBlackhole()
    {
        if(blackholeUnlockButton.unlocked)
        {
            blackholeUnlocked= true;
        }
        else
        {
            blackholeUnlocked = false;
        }
    }
    public override void CheckUnlock()
    {
        base.CheckUnlock();
        UnclockBlackhole();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        blackholeUnlockButton.GetComponent<Button>().onClick.AddListener(UnclockBlackhole);
    }

    protected override void Update()
    {
        base.Update();
    }
    public bool SkillCompleted()
    {
        if(!blackholeController) return false;

        if(blackholeController.playerCanExitState)
        {
            blackholeController = null;
            return true;
        }
        return false;
    }
    public float GetRadius()
    {
        return maxSize/2;
    }
}
