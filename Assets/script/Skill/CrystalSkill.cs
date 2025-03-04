using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float CrystalDuration = 5f;
    private GameObject crystal;

    [SerializeField] private UISkillTreeSlot uncloneInsteadButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [SerializeField] private UISkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked{get;private set;}

    
    [SerializeField] private bool canExplode;
    [SerializeField] private UISkillTreeSlot unlockExplodeButton;


    [SerializeField] private float movespeed;
    [SerializeField] private bool canMove;
    [SerializeField] private UISkillTreeSlot unlockMoveButton;

    [Header("Multi stacking crystal")]
    [SerializeField] private UISkillTreeSlot unlockStackingButton;
    [SerializeField] private bool canStack;
    [SerializeField] private float multStackDuration;
    [SerializeField] private int maxStack;
    [SerializeField] private float useTimeWondow;
    [SerializeField] private List<GameObject> crystalStack = new List<GameObject>();


    protected override void Start()
    {
        base.Start();
        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockExplodeButton.GetComponent<Button>().onClick.AddListener(UnlockExplode);
        unlockMoveButton.GetComponent<Button>().onClick.AddListener(UnlockMove);
        unlockStackingButton.GetComponent<Button>().onClick.AddListener(UnlockStacking);
        uncloneInsteadButton.GetComponent<Button>().onClick.AddListener(UnlockUncloneInstead);
    }
    public override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockCrystal();
        UnlockExplode();
        UnlockMove();
        UnlockStacking();
        UnlockUncloneInstead();

    }
    private void UnlockCrystal()
    {
        if(unlockCrystalButton.unlocked)
        {
            crystalUnlocked = true;
        }
        else
        {
            crystalUnlocked = false;
        }
    }
    private void UnlockUncloneInstead()
    {
        if(uncloneInsteadButton.unlocked)
        {
            cloneInsteadOfCrystal = true;
        }
        else
        {
            cloneInsteadOfCrystal = false;
        }
    }
    private void UnlockExplode()
    {
        if(unlockExplodeButton.unlocked)
        {
            canExplode = true;
        }
        else
        {
            canExplode = false;
        }
    }
    private void UnlockMove()
    {
        if(unlockMoveButton.unlocked)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }
    }
    private void UnlockStacking()
    {
        if(unlockStackingButton.unlocked)
        {
            canStack = true;
        }
        else
        {
            canStack = false;
        }
    }
   


    public override void UseSkill()
    {
       base.UseSkill();
       
        if(CanuseCrystal()) return;

        if(crystal == null)
        {
            CreateCrystal();
        }
        else
        {
            if(canMove) return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = crystal.transform.position;
            crystal.transform.position = playerPos;
            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(crystal.transform, Vector3.zero);
                Destroy(crystal);
            }
            else

            {
                crystal.GetComponent<CrystalSkillController>().FinishCrystal();
            }
            //Destroy(crystal);
        }
    }

    public void CreateCrystal()
    {
        //crystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        //crystal = PoolManager.instance.GetControllerFromPool("Crystal");
        //crystal.transform.position = player.transform.position;
        //crystal.transform.rotation = Quaternion.identity;
        crystal = PoolMgr.Instance.GetObj("Crystal", player.transform.position, Quaternion.identity);
        CrystalSkillController crystalController = crystal.GetComponent<CrystalSkillController>();
        crystalController.SetCrystal(CrystalDuration, movespeed, canMove, canExplode, closeEnemy(crystal.transform));
       // crystalController.ChosseRandomEnemy();
    }
    public void CurrentCrystalChooseRandTarget()=> crystal.GetComponent<CrystalSkillController>().ChosseRandomEnemy();

    private void RefilCrystal()
    {
        int count = maxStack - crystalStack.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject crystal = crystalPrefab;
                //ProxyResourceFactory.Instance.Factory.GetControllers("Crystal");
            crystalStack.Add(crystal);
            //Destroy(crystal);
        }
    }
    private bool CanuseCrystal()
    {
        if(canStack)
        {
            if (crystalStack.Count > 0)
            {
                if(crystalStack.Count == maxStack)
                {
                    Invoke("ResetAbility", useTimeWondow);
                }

                cooldoun = 0;
                GameObject crystal = crystalStack[crystalStack.Count - 1];
                //AudioManager.instance.PlaySFX(30, null);
                //GameObject newcrystal = Instantiate(crystal, player.transform.position, Quaternion.identity);
                GameObject newcrystal = PoolMgr.Instance.GetObj("Crystal", player.transform.position, Quaternion.identity);
                //PoolManager.instance.GetControllerFromPool("Crystal");
                newcrystal.transform.position = player.transform.position;
                newcrystal.transform.rotation = Quaternion.identity;
                newcrystal.gameObject.GetComponent<CrystalSkillController>().
                    SetCrystal(multStackDuration, movespeed, canMove, canExplode, closeEnemy(newcrystal.transform));
                crystalStack.Remove(crystal);
                if (crystalStack.Count <= 0)
                {
                    cooldoun=multStackDuration;
                    RefilCrystal();
                }
            }
            return true;
        }
        return false;
    }
    private void ResetAbility()
    {
        if(cooldoun>0) return;

        cooldoun=multStackDuration;
        RefilCrystal();
    }
}
