using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{
    public bool dodgeUnlocked { get; private set; }
    public bool dodgeMirageUnlocked { get; private set; }

    [SerializeField] private UISkillTreeSlot unlockDodgeButton;
    [SerializeField] private UISkillTreeSlot unlockDodgeMirageButton;

    [SerializeField] private int experienceCost;

    protected override void Start()
    {
        base.Start();
        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockDodgeMirageButton.GetComponent<Button>().onClick.AddListener(UnlockDodgeMirage);
    }
    public override void CheckUnlock()
    { 
        UnlockDodge();
        UnlockDodgeMirage();
    }
    public void UnlockDodge()
    {
        if (unlockDodgeButton.unlocked&&!dodgeUnlocked)
        {
            //Debug.Log(player);
            player.stats.evasion.AddModifier(experienceCost);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }
        else
        {
            player.stats.evasion.RemoveModifier(experienceCost);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = false;
        }
    }
    public void UnlockDodgeMirage()
    {
        if (unlockDodgeMirageButton.unlocked)
        {
            dodgeMirageUnlocked = true;
        }
        else
        {
            dodgeMirageUnlocked = false;
        }
    }
    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
        {
            int facingDirection = -1;
            if (Random.Range(0, 2)==0)
            {
                facingDirection = 1;
            }


            //SkillManager.instance.clone.CreateClone(player.transform,new Vector3(2*player.facingDirection,0));
            SkillManager.instance.clone.CreateClone(closeEnemy(player.transform), new Vector3(2 * facingDirection, 0));
        }
    }
}
