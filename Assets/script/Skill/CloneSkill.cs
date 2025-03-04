using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    private float attackMultiplier;

    //[SerializeField] private GameObject clone;
    [SerializeField] private float cloneDuration;

    [SerializeField] private UISkillTreeSlot canCloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canCloneAttack;

    [SerializeField] private UISkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneMultiplier;
   public bool canAggresiveClone { get; private set; }


    //[SerializeField] private bool createCloneOnDashStart=true;
    //[SerializeField] private bool createCloneOnDashEnd=true ;
    //[SerializeField] private bool createCloneOnCounterAttack=true;

    [SerializeField] private UISkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiplecloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
   

    [SerializeField] private UISkillTreeSlot crystalUnlockButton;
    [SerializeField] private bool craystalInseadOfClone;

    protected override void Start()
    {
        base.Start();
        canCloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultipleClone);
        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);

    }
    public override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultipleClone();
        UnlockCrystal();
    }

    private void UnlockCloneAttack()
    {
        if(canCloneAttackUnlockButton.unlocked)
        {
            canCloneAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
        else
        {
            canCloneAttack = false;
            attackMultiplier = 1;
        }
    }
    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            canAggresiveClone = true;
            attackMultiplier = aggresiveCloneMultiplier;
        }
        else
        {
            canAggresiveClone = false;
            attackMultiplier = 1;
        }
    }
    private void UnlockMultipleClone()
    {
        if (multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiplecloneAttackMultiplier;
        }
        else
        {
            canDuplicateClone = false;
            attackMultiplier = 1;
        }
    }
        
    private void UnlockCrystal()
    {
        if (crystalUnlockButton.unlocked)
        {
            craystalInseadOfClone = true;

        }
        else
        {
            crystalUnlockButton.unlocked = false;
        }
    }


    public void CreateClone(Transform transform,Vector3 position,bool canDuplicate=false)
    {
        
        if (craystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            SkillManager.instance.crystal.CurrentCrystalChooseRandTarget();
           // return;
        }

        //GameObject newclone = Instantiate(clone);
        //  GameObject newclone = PoolManager.instance.GetControllerFromPool("clone");
        GameObject newclone = PoolMgr.Instance.GetObj("clone", transform.position, Quaternion.identity);
            //newclone.transform.position = transform.position;
            newclone.GetComponent<CloneSkillController>().SetupClone(transform,
                cloneDuration, canCloneAttack, position, closeEnemy(player.transform), canDuplicateClone, chanceToDuplicate, attackMultiplier, canDuplicate);
        
    }

    
    public void CreateCloneOnCounterAttack(Transform transform)
    {
       // if (createCloneOnCounterAttack)
        {
           // CreateClone(transform,new Vector3(2*player.facingDirection,0,0));
           StartCoroutine(CloneDuration(transform, new Vector3(2 * player.facingDirection, 0, 0)));
        }
    }
   private IEnumerator CloneDuration(Transform transform,Vector3 position)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(transform, position);
    }
}
