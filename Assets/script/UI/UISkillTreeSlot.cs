using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    private UI ui;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color skillColor;

    [SerializeField] private int skillPrice;


    public bool unlocked;

    [SerializeField] private UISkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UISkillTreeSlot[] shouldBeLocked;
    [SerializeField] private Image skillImage;


    private void OnValidate()
    {
        gameObject.name = skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UnlockSkillSlot());

    }
    // Start is called before the first frame update



    void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();
        skillImage.color = skillColor;

        if (unlocked)
        {
            skillImage.color = Color.white;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UnlockSkillSlot()
    {
        if (unlocked)
        {
            PlayerManager.instance.AddMoney(skillPrice);
            unlocked = false;
            skillImage.color = skillColor;
            AudioManager.instance.PlaySFX(21, null);
            //SkillManager.instance.CheckSkillUnlock();

            return;
        }
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                AudioManager.instance.PlaySFX(17, null);
                return;

            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                AudioManager.instance.PlaySFX(17, null);
                return;
            }

        }
        
        if (!PlayerManager.instance.HaveEnoughMoney(skillPrice)) return;
        AudioManager.instance.PlaySFX(21, null);
        unlocked = true;
        skillImage.color = Color.white;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowToolTip(skillDescription, skillName, skillPrice);


        Vector2 mosePosition = Input.mousePosition;
        float xOffset = 0;
        float yOffset = 0;
        if (mosePosition.x > Screen.width / 2)
        {
            xOffset = -200;
        }
        else
        {
            xOffset = 200;
        }
        if (mosePosition.y > Screen.height / 2)
        {
            yOffset = -200;
        }
        else
        {
            yOffset = 200;
        }
        ui.skillTooltip.transform.position = new Vector3(mosePosition.x + xOffset, mosePosition.y + yOffset, 0);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideToolTip();
    }

    public void LoadData(GameData data)
    {
        //throw new System.NotImplementedException();
        if (data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
       SkillManager.instance.CheckSkillUnlock();
    }

    public void SaveData(ref GameData data)
    {
        if (data.skillTree.TryGetValue(skillName, out bool value))
        {
            data.skillTree.Remove(skillName);
            data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            data.skillTree.Add(skillName, unlocked);
        }
    }
}
