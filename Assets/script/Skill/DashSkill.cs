using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill
{
    public bool dashUnlocked { get; private set; }
    public bool cloneOnDashUncloked { get; private set; }
    public bool cloneOnArrivalUnlocked { get; private set; }
    [SerializeField] private UISkillTreeSlot dashUnlockButton;
    [SerializeField] private UISkillTreeSlot cloneOnDashUnlockButton;
    [SerializeField] private UISkillTreeSlot cloneOnArrivalUnlockButton;
    public override void UseSkill()
    {
        base.UseSkill();
    }
    protected override void Start()
    {
        base.Start();
        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrival);
    }
    public override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrival();

    }

    private void UnlockDash()
    {
        if (dashUnlockButton.unlocked)
        {
            dashUnlocked = true;
        }
        else
        {
            dashUnlocked = false;
        }
        
    }
    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
        {
            cloneOnDashUncloked = true;
        }
        else
        {
            cloneOnDashUncloked = false;
        }
    }
    private void UnlockCloneOnArrival()
    {
        if (cloneOnArrivalUnlockButton.unlocked)
        {
            cloneOnArrivalUnlocked = true;
        }
        else
        {
            cloneOnArrivalUnlocked = false;
        }
    }
    public void CreateCloneOnDashStart()
    {
              
        if (Vector2.Distance(closeEnemy(player.transform).position,player.transform.position) <= 3 
            && closeEnemy(player.transform).position != player.transform.position)

        {
            if (cloneOnDashUncloked)
            {
                SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
            }
        }
    }
    public void CreateCloneOnDashEnd()
    {
        if (Vector2.Distance(closeEnemy(player.transform).position, player.transform.position) <= 3
            && closeEnemy(player.transform).position!= player.transform.position)
        {
            if (cloneOnArrivalUnlocked)
            {
                SkillManager.instance.clone.CreateClone(player.transform, Vector3.zero);
            }
        }
    }
}
