using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}
public class SwordSkill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private UISkillTreeSlot bounceUnlockButton;
    [SerializeField] private int amountOfBounces;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce info")]
    [SerializeField] private UISkillTreeSlot pierceUnlockButton;
    [SerializeField] private int amountOfPierce;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private UISkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;

    [Header("Sword Skill")]
    [SerializeField] private UISkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject sword;
    [SerializeField] private Vector2 launchDirection;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive skills")]
    [SerializeField] private UISkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UISkillTreeSlot volnurableUnlockButton;
   // private bool volnurableUnlocked;
    public bool volnurableUnlocked { get; private set; }


    private Vector2 finalDirection;

    [Header("Aim dots")]
    [SerializeField] private int numDots;
    [SerializeField] private float dotDistance;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();
        CreateDots();
        SetupGravity();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounce);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierce);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpin);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        volnurableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVolnurable);


    }
    public override void CheckUnlock()
    {
        base.CheckUnlock();
        UnlockSword();
        UnlockBounce();
        UnlockSpin();
        UnlockPierce();
        UnlockTimeStop();
        UnlockVolnurable();

    }

    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
        {
            timeStopUnlocked = true;
        }
        else
        {
            timeStopUnlocked = false;
        }
    }
    private void UnlockVolnurable()
    {
        if (volnurableUnlockButton.unlocked)
        {
            volnurableUnlocked = true;
        }
        else
        {
            volnurableUnlocked = false;
        }
    }
    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;

        }
        else
        {
            swordType = SwordType.Regular;
            swordUnlocked = false;
        }
    }
    private void UnlockPierce()
    {
        if (pierceUnlockButton.unlocked)
        {
            swordType = SwordType.Pierce;
        }
        else if (!spinUnlockButton.unlocked&&!bounceUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
        }
    }
    private void UnlockSpin()
    {
        if (spinUnlockButton.unlocked)
        {
           
            swordType = SwordType.Spin;
        }
        else if (!pierceUnlockButton.unlocked && !bounceUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
        }
    }
    private void UnlockBounce()
    {
        if (bounceUnlockButton.unlocked)
        {
           
            swordType = SwordType.Bounce;
        }
        else if (!pierceUnlockButton.unlocked && !spinUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
        }
    }

      



    private void SetupGravity()
    {
        if(swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
        }
        else if(swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
        }
        else if(swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
        }
    }
    protected override  void Update()
    {
        base.Update();
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            finalDirection = new Vector2(AimDirection().x*launchDirection.x, AimDirection().y*launchDirection.y);

           
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            for (int i = 0; i < numDots; i++)
            {
                dots[i].transform.position = GetDotPosition(i * dotDistance);
            }
        }
    }
   
    public void CreateSword()
    {
        GameObject swordClone = Instantiate(sword, player.transform.position, transform.rotation);
        SwordSkillController swordController = swordClone.GetComponent<SwordSkillController>();
        if (swordType == SwordType.Bounce)
        {
            //swordGravity= bounceGravity;
            swordController.SetupBounce(true,amountOfBounces, bounceSpeed);
        }
        else if (swordType == SwordType.Pierce)
        {
            swordController.SetupPierce(amountOfPierce);
        }
        else if (swordType == SwordType.Spin)
        {
            swordController.SetupSpin(true,spinDuration,maxTravelDistance,hitCooldown);
           // Debug.Log(spinDuration);
        }

        swordController.SetupSword(finalDirection, swordGravity, player, freezeTimeDuration, returnSpeed);
        player.AssignNewSword(swordClone);
        DotsActive(false);
    }
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePosition - playerPosition).normalized;
    }

    public void CreateDots()
    {
        dots = new GameObject[numDots];
        for (int i = 0; i < numDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, transform.rotation, dotParent);
            dots[i].SetActive(false);
        }
    }
    public void DotsActive(bool active)
    {
        for (int i = 0; i < numDots; i++)
        {
            dots[i].SetActive(active);
        }
    }
    public Vector2 GetDotPosition(float index)
    {
        Vector2 Position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchDirection.x,
            AimDirection().normalized.y * launchDirection.y) * index + .5f * (Physics2D.gravity * swordGravity * index * index);
        return Position;
    }
}
