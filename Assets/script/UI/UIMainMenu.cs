using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIMainMenu : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private Selectable _firstSelected; // ��ʼѡ����
    [SerializeField] private float _navDelay = 0.2f; // ������Ӧ���

    private Selectable _lastSelectedBeforePopup; // ����ǰ���ѡ�еĶ���
    [Header("Scene Settings")]
    // [SerializeField] private string sceneName;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private UIFadeScreen fadeScreen;
    public AssetReference currentSceneSO;
    [Header("UI Components")]
    [SerializeField] private GameObject saveSlotPrefab;  // �浵��λԤ����
    [SerializeField] private Transform saveSlotContainer; // �浵�б�����
    [SerializeField] private Button newGameButton;       // ȫ���½���Ϸ��ť
    [SerializeField] private Button ReturnButton;
    [SerializeField] private GameObject confirmDeletePanel; // ɾ��ȷ�����
    [SerializeField] private TMP_Text emptySlotHint;     // �մ浵��ʾ�ı�
    public int maxSaveSlots = 5; // ���浵��λ��
                                 // ��UIMainMenu����������³�Ա����
    [Header("New Game Settings")]
    [SerializeField] private GameObject overwriteConfirmPanel; // ����ȷ�����
    [SerializeField] private TMP_Text overwriteConfirmText;
    private int _newGameSlotIndex = 1; // Ĭ��ʹ�õ�һ���浵λ

    public GameObject Saves;
    public GameObject Loads;


    private int selectedSlotIndex = -1; // ��ǰѡ�еĴ浵��λ
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        RefreshSaveSlots();
        confirmDeletePanel.SetActive(false);
        Saves?.SetActive(false);
        Loads.SetActive(false);
        // �滻ԭ��EventSystem����
        StartCoroutine(SetInitialFocus());

        // ��ӵ�������
        SetupButtonNavigation();
        //if (!SaveManager.instance.HasSaveData())
        //{
        //    continueButton.SetActive(false);
        //}
        var inputModule = EventSystem.current.currentInputModule as StandaloneInputModule;
        if (inputModule != null)
        {
            inputModule.inputActionsPerSecond = 1f / _navDelay;
        }
        continueButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ContinueGame();
        });
       ReturnButton.GetComponent<Button>().onClick.AddListener(() => {
            Loads.SetActive(false);
            EnableMainMenuNavigation(this.gameObject);
           StartCoroutine(SetPopupFocus(continueButton.GetComponent<Selectable>()));
       });

    }
    // ��ӵ����ٶȿ���
    private void Update()
    {
        
    }
    

    // �޸����е�NewGame����
    public void NewGame()
    {
        bool hasAnySave = CheckExistingSaves();

        if (!hasAnySave)
        {
            // û�д浵ֱ�Ӵ�������Ϸ
            CreateNewSave(_newGameSlotIndex);
        }
        else
        {
            // Saves.SetActive(true);
            // ��ʾ����ȷ�����
            ShowOverwriteConfirm();
        }
    }

    // �������浵����
    private bool CheckExistingSaves()
    {
        for (int i = 1; i <= maxSaveSlots; i++)
        {
            if (SaveManager.instance.HasSaveData(i))
            {
                return true;
            }
        }
        return false;
    }

    // ��������ȷ�Ϸ���
    private void ShowOverwriteConfirm()
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        _lastSelectedBeforePopup = (currentSelected != null) ?
            currentSelected.GetComponent<Selectable>() :
            _firstSelected; // ����ǰ��ѡ������˵���ʼ��

        overwriteConfirmPanel.SetActive(true);
        overwriteConfirmText.text = "��⵽���д浵���Ƿ񸲸����н��ȣ�";

        // ����ȷ����尴ť
        Button btn_Yes = overwriteConfirmPanel.transform.Find("btn_ConfirmYes").GetComponent<Button>();
        Button btn_No = overwriteConfirmPanel.transform.Find("btn_ConfirmNo").GetComponent<Button>();
        btn_Yes.GetComponentInChildren<TMP_Text>().text = "ȷ��";
        btn_No.GetComponentInChildren<TMP_Text>().text = "ȡ��";
        btn_Yes.onClick.RemoveAllListeners();
        btn_Yes.onClick.AddListener(() => {
            ForceNewGame();
            overwriteConfirmPanel.SetActive(false);
        });

        btn_No.onClick.RemoveAllListeners();
        btn_No.onClick.AddListener(() => {
            overwriteConfirmPanel.SetActive(false);
            // �������ָ����˵�����
            EnableMainMenuNavigation(this.gameObject);
            StartCoroutine(RestoreFocusAfterPopup());
        });
        DisableMainMenuNavigation(this.gameObject);
        // ���õ���
        SetupHorizontalNavigation(btn_Yes.GetComponent<Selectable>(), btn_No.GetComponent<Selectable>());
        StartCoroutine(SetPopupFocus(btn_Yes.GetComponent<Selectable>()));
        EnableMainMenuNavigation(overwriteConfirmPanel);
    }
   
    // ǿ���½���Ϸ����
    private void ForceNewGame()
    {
        for (int i = 1; i <= maxSaveSlots; i++)
        {
            SaveManager.instance.DeleteSavedData(i);
        }
        CreateNewSave(_newGameSlotIndex);
    }

    IEnumerator SetInitialFocus()
    {
        yield return new WaitForEndOfFrame();
        bool hasAnySave = CheckExistingSaves();

        if (!hasAnySave)
        {  //  StartCoroutine(SetPopupFocus(continueButton.GetComponent<Selectable>()));
            EventSystem.current.SetSelectedGameObject(_firstSelected?.gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(continueButton?.gameObject);
        }
    }

    private void SetupButtonNavigation()
    {
        // �����浼��
        var newGameNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = continueButton.GetComponent<Selectable>(),
             selectOnUp = _firstSelected //GetComponentInChildren<Button>() // ����ʵ�ʲ��ֵ���
        };
        newGameButton.navigation = newGameNav;

        // ��̬��λ������RefreshSaveSlots�д���
    }
    // �������˵�����
    private void DisableMainMenuNavigation(GameObject popup)
    {
        foreach (var selectable in popup.GetComponentsInChildren<Selectable>())
        {
            selectable.interactable = false;
        }
    }
    // �����رպ�ָ�����
    private void EnableMainMenuNavigation(GameObject popup)
    {
        foreach (var selectable in popup.GetComponentsInChildren<Selectable>())
        {

            selectable.interactable = true;
        }
       
    }
    // ˢ�����д浵��λUI
    public void RefreshSaveSlots()
    {
        // ������в�λ
        foreach (Transform child in saveSlotContainer)
        {
            Destroy(child.gameObject);
        }

        bool hasAnySave = false;

        Selectable previousSlotButton = null;
        // ����3���浵��λ��ʾ����
        for (int i = 0; i < maxSaveSlots; i++)
        {
            GameObject slot = Instantiate(saveSlotPrefab, saveSlotContainer);
            int slotIndex = i + 1; // ��λ��1��ʼ

            // ��ȡԪ���ݣ�����GameData������ӣ�
            bool hasSave = SaveManager.instance.HasSaveData(slotIndex);
            string saveTime = hasSave ?
                SaveManager.instance.GetSaveMetadata(slotIndex).saveTime :
                "�մ浵";
            TimeSpan timeSpan = hasSave ?
                TimeSpan.FromSeconds(SaveManager.instance.GetSaveMetadata(slotIndex)._totalPlaytimeSeconds) :
                TimeSpan.Zero;
            int playerLevel = hasSave ?
                SaveManager.instance.GetSaveMetadata(slotIndex).level :
                0;

            // ����UIԪ��
            slot.transform.Find("txt_SlotNumber").GetComponent<TMP_Text>().text = $"�浵λ {slotIndex}";
            slot.transform.Find("txt_SaveTime").GetComponent<TMP_Text>().text 
                = $"����ʱ��: {saveTime}    ��Ϸʱ��:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            slot.transform.Find("txt_PlayerLevel").GetComponent<TMP_Text>().text = $"�ȼ�: {playerLevel}";

            // ��ť�¼���
            Button btn_Load = slot.transform.Find("btn_Load").GetComponent<Button>();
            Button btn_Delete = slot.transform.Find("btn_Delete").GetComponent<Button>();

            if (hasSave)
            {
                btn_Load.GetComponentInChildren<TMP_Text>().text= $"����";
                btn_Delete.GetComponentInChildren<TMP_Text>().text= $"ɾ��";
                btn_Load.onClick.AddListener(() => OnSlotSelected(slotIndex));
                btn_Delete.onClick.AddListener(() => ShowDeleteConfirm(slotIndex));
                hasAnySave = true;
            }
            else
            {
                btn_Load.GetComponentInChildren<TMP_Text>().text = "��ʼ����Ϸ";
                btn_Load.onClick.AddListener(() => CreateNewSave(slotIndex));
                
                btn_Delete.gameObject.SetActive(false);
                // ���������ء���ť���ҵ���
                var loadSelectable = btn_Load.GetComponent<Selectable>();
                var loadNav = loadSelectable.navigation;
                loadNav.selectOnRight = null; // ����ҵ���
                loadSelectable.navigation = loadNav;
            }
            // ��������
            var slotSelectable = slot.GetComponent<Selectable>();
            if (previousSlotButton != null)
            {
                SetupVerticalNavigation(previousSlotButton, slotSelectable);
            }
            previousSlotButton = slotSelectable;

            // ��ť�ڲ�����
            var loadButton = btn_Load.GetComponent<Selectable>();
            var deleteButton = btn_Delete.GetComponent<Selectable>();
            SetupHorizontalNavigation(loadButton, deleteButton);
        }
        // ��βѭ������
        if (saveSlotContainer.childCount > 0)
        {
            var first = saveSlotContainer.GetChild(0).GetComponent<Selectable>();
            var last = saveSlotContainer.GetChild(saveSlotContainer.childCount - 1).GetComponent<Selectable>();
            SetupVerticalNavigation(last, first);
        }
        emptySlotHint.gameObject.SetActive(!hasAnySave);
    }
    private void SetupVerticalNavigation(Selectable upper, Selectable lower)
    {
        if (upper == null && lower == null) return;
        var upperNav = upper.navigation;
        upperNav.selectOnDown = lower;
        upper.navigation = upperNav;

        var lowerNav = lower.navigation;
        lowerNav.selectOnUp = upper;
        lower.navigation = lowerNav;
    }

    private void SetupHorizontalNavigation(Selectable left, Selectable right)
    {
        if (left == null && right == null) return;
        var leftNav = left.navigation;
        leftNav.selectOnRight = right;
        left.navigation = leftNav;

        var rightNav = right.navigation;
        rightNav.selectOnLeft = left;
        right.navigation = rightNav;
    }

    // ѡ�д浵��λ
    private void OnSlotSelected(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        StartCoroutine(LoadSceneWithFadeEffect(0.5f));
    }
    // �����´浵
    private void CreateNewSave(int slotIndex)
    {
        SaveManager.instance.DeleteSavedData(slotIndex); // ȷ���ɾ�״̬
       // SaveManager.instance.SaveGame(slotIndex);         // ��ʼ����
        RefreshSaveSlots();
        OnSlotSelected(slotIndex);
    }

    // ��ʾɾ��ȷ�����
    private void ShowDeleteConfirm(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        confirmDeletePanel.SetActive(true);
        confirmDeletePanel.transform.Find("txt_Confirm").GetComponent<TMP_Text>().text =
            $"ȷ��Ҫɾ���浵λ {slotIndex} �Ľ�����";
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        _lastSelectedBeforePopup = (currentSelected != null) ?
            currentSelected.GetComponent<Selectable>() :
            _firstSelected; // ����ǰ��ѡ������˵���ʼ��
        // �������������˵�����
        DisableMainMenuNavigation(this.gameObject);
        EnableMainMenuNavigation(confirmDeletePanel);
        Button btn_Yes = confirmDeletePanel.transform.Find("btn_ConfirmYes").GetComponent<Button>();
        Button btn_No = confirmDeletePanel.transform.Find("btn_ConfirmNo").GetComponent<Button>();
        btn_Yes.GetComponentInChildren<TMP_Text>().text = "ȷ��";
        btn_No.GetComponentInChildren<TMP_Text>().text = "ȡ��";
        btn_Yes.onClick.AddListener(ConfirmDelete);
        btn_No.onClick.AddListener(CancelDelete);

       // _lastSelectedBeforePopup = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
       
        // ����ȷ����嵼��
        var yesButton = confirmDeletePanel.transform.Find("btn_ConfirmYes").GetComponent<Selectable>();
        var noButton = confirmDeletePanel.transform.Find("btn_ConfirmNo").GetComponent<Selectable>();
        SetupHorizontalNavigation(yesButton, noButton);

        StartCoroutine(SetPopupFocus(yesButton));
    }
    IEnumerator SetPopupFocus(Selectable firstSelected)
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }

    // ȷ��ɾ���浵
    public void ConfirmDelete()
    {
        if (selectedSlotIndex != -1)
        {
            SaveManager.instance.DeleteSavedData(selectedSlotIndex);
            RefreshSaveSlots();
            confirmDeletePanel.SetActive(false);
            // �������������˵�����
            EnableMainMenuNavigation(Loads);
            // StartCoroutine(RestoreFocusAfterPopup());
            StartCoroutine( SetPopupFocus(ReturnButton.GetComponent<Selectable>()));
        }
    }

    // ȡ��ɾ��
    public void CancelDelete()
    {
        
        confirmDeletePanel.SetActive(false);
        selectedSlotIndex = -1;
        // �������������˵�����
        EnableMainMenuNavigation(Loads);
        StartCoroutine(RestoreFocusAfterPopup());
    }
    IEnumerator RestoreFocusAfterPopup()
    {
        yield return null;
      
        if (_lastSelectedBeforePopup != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelectedBeforePopup.gameObject);
            _lastSelectedBeforePopup = null; // �����ÿղ���
        }
    }
   
    IEnumerator LoadSceneWithFadeEffect(float fadeTime)
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.FadeOut();
       // Debug.Log("Loading Scene..." + selectedSlotIndex);
        // ���ݴ浵��λ��������Ϸ����
        PlayerPrefs.DeleteKey("SelectedSaveSlot");
        PlayerPrefs.SetInt("SelectedSaveSlot", selectedSlotIndex);
        PlayerPrefs.Save();
      
        yield return new WaitForSeconds(fadeTime);
       
        Addressables.LoadSceneAsync(currentSceneSO);
        //currentSceneSO.LoadSceneAsync(LoadSceneMode.Additive, true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void ContinueGame()
    {
        Loads.SetActive(true);
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        _lastSelectedBeforePopup = (currentSelected != null) ?
            currentSelected.GetComponent<Selectable>() :
            _firstSelected; // ����ǰ��ѡ������˵���ʼ��
        DisableMainMenuNavigation(this.gameObject);
        EnableMainMenuNavigation(Loads);
        StartCoroutine(SetPopupFocus(ReturnButton.GetComponent<Selectable>()));
       // StartCoroutine(SetPopupFocus(yesButton));
        //SceneManager.LoadScene(sceneName);
        //StartCoroutine(LoadScenceWithFadeEffect(0.1f));
    }
    
}
