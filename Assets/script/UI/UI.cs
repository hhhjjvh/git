using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour, ISaveManager
{
    public static UI instance;

    [SerializeField] private GameObject characaterUI;
    [SerializeField] private GameObject SkillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionUI;
    [SerializeField] private GameObject inGameUI;
    public ChargeUI ChargeUI;

    public GameObject ShopUI;
    public UIFadeScreen fadeScreen;
    public GameObject endText;
    public GameObject restButton;
    public GameObject ExitButton;



    public UIItemTooltip itemTooltip;
    public UIStatToolTip statTooltip;
    public UICraftWindow craftWindow;
    public UIShopWindow shopWindow;
    public UISkillToolTip skillTooltip;

    public UIItemText itemText;

    [SerializeField] private UIVolumeSlider[] volumeSliders;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SwithTo(SkillTreeUI);
        fadeScreen.gameObject.SetActive(true);
      
    }
   

    // Start is called before the first frame update
    void Start()
    {
        //itemTooltip = GetComponentInChildren<UIItemTooltip>();
        //statTooltip = GetComponentInChildren<UIStatToolTip>();
        SwithTo(inGameUI);
        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
        ShopUI.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerManager.instance.player.GetComponent<CharacterStats>().isDead|| !PlayerManager.instance.player.canMove) { return; }


        if (InputManager.Instance.canPause)
        {
            SwitchWithKeyTo(characaterUI);
            InputManager.Instance.canPause= false;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchWithKeyTo(SkillTreeUI);

        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SwitchWithKeyTo(craftUI);
            craftWindow.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SwitchWithKeyTo(optionUI);
        }
        /*
        if (GameManager.instance != null)
        {
            if (ShopUI.activeInHierarchy||characaterUI.activeInHierarchy||SkillTreeUI.activeInHierarchy||craftUI.activeInHierarchy||optionUI.activeInHierarchy)
            {
                GameManager.instance.PauseGame(true);
            }
            else
            {
                GameManager.instance.PauseGame(false);
               
            }

        }
        */
        if (!inGameUI.activeInHierarchy)
        {
            CheckInGameUI();
        }
        
    }
    public void SwithToShop()
    {
        SwithTo(ShopUI);
        shopWindow.gameObject.SetActive(false);

    }
    public void SwithTo(GameObject menu)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            bool isFadeScreen = transform.GetChild(i).GetComponent<UIFadeScreen>() != null;
            if (!isFadeScreen)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

        }

        if (menu != null)
        {
            menu.SetActive(true);
        }

        if (GameManager.instance != null)
        {
            if (menu == inGameUI)
            {
                GameManager.instance.PauseGame(false);
            }
            else
            {
                GameManager.instance.PauseGame(true);
            }

        }
    }
    public void SwitchWithKeyTo(GameObject menu)
    {
        AudioManager.instance.StopAllSFX();
        if (menu != null && menu.activeSelf)
        {
            menu.SetActive(false);
            CheckInGameUI();
            return;
        }
        SwithTo(menu);
    }

    private void CheckInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UIFadeScreen>() == null)
                return;
        }
        SwithTo(inGameUI);
    }

    public void SwithOnSceneEnd()
    {
       
        fadeScreen.FadeOut();
        StartCoroutine(ShowEnd());
    }

    IEnumerator ShowEnd()
    {
        yield return new WaitForSeconds(1f);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        restButton.SetActive(true);
        ExitButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restButton);
        Time.timeScale = 0;

    }

    public void RestartGameButton()
    {
       // StartCoroutine(UnLoadScence1());
        GameManager.instance.RestartScence();
      //SaveManager.instance.LoadGame();

    }
    public void ExitGameButton()
    {
       // StartCoroutine(UnLoadScence2());
        GameManager.instance.ExitGame();
    }
    IEnumerator UnLoadScence1()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.RestartScence();
        
    }
    IEnumerator UnLoadScence2()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.ExitGame();
       
    }

    public void LoadData(GameData data)
    {
        foreach (KeyValuePair<string, float> pair in data.volumeSetting)
        {
            foreach (UIVolumeSlider volumeSlider in volumeSliders)
            {
                if (volumeSlider.parametr == pair.Key)
                {
                    volumeSlider.LoadSlider(pair.Value);
                }
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.volumeSetting.Clear();
        foreach (UIVolumeSlider volumeSlider in volumeSliders)
        {
            data.volumeSetting.Add(volumeSlider.parametr, volumeSlider.slider.value);
        }
    }
}
