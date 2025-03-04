using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [Header("UI Components")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText, nameText;
    private enum State { Inactive, Scrolling, Choosing }
    private State currentState;

    private int currentLine = 0;

    private bool isScrolling = false;
    [SerializeField] private float scrollSpeed;
    private float defaultScrollSpeed;
    private NPCAbout npc;
    [Header("Dialogue Settings")]
    public DialogueSO currentDialogue;  // 当前对话的数据
    private string[] splitDialogueLines;  // 存储分割后的对话内容
    private Dictionary<int, UnityEvent> eventMap;
    [Header("Branching Options")]
    public GameObject choicePanel;
    // public GameObject choiceButtonPrefab;
    private List<GameObject> currentChoices = new List<GameObject>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }

        }
        dialogueBox.SetActive(false);
        choicePanel.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        defaultScrollSpeed = scrollSpeed;
        //dialogueText.text = dialogueLines[currentLine];
        if (currentDialogue != null && !string.IsNullOrEmpty(currentDialogue.dialogueData))
        {
            splitDialogueLines = currentDialogue.cachedLines;
            dialogueText.text = splitDialogueLines[currentLine];
        }
        if (currentDialogue != null && currentDialogue.nodes[0].content != null)
        {
            splitDialogueLines = currentDialogue.nodes[0].content;
            dialogueText.text = splitDialogueLines[currentLine];
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != State.Scrolling) return;
        if (dialogueBox.activeInHierarchy && !choicePanel.activeInHierarchy && splitDialogueLines != null)
        {
            if (InputManager.Instance.canInteract)
            {
                InputManager.Instance.canInteract = false;
                //Debug.Log("Interact with NPC");
                scrollSpeed = defaultScrollSpeed;
                if (!isScrolling)
                {

                    currentLine++;
                    if (currentLine >= splitDialogueLines.Length)
                    {
                        EndDialogue();
                    }
                    else
                    {

                        CheckName();
                        if (currentDialogue.systemType == DialogueSO.SystemType.NodeBased)
                        {
                            //Debug.Log("Node Based");
                            StartCoroutine(ProcessNodeSystem());
                        }
                        else
                        {
                            //  Debug.Log("Legacy System");
                            StartCoroutine(ProcessLegacySystem());
                        }

                    }
                }
                else
                {
                    StopAllCoroutines();
                    dialogueText.text = splitDialogueLines[currentLine];
                    isScrolling = false;
                }

            }
        }

    }

    public void StartDialogue(DialogueSO dialogue, NPCAbout npc)
    {

        PlayerManager.instance.player.canMove = false;
        this.npc = npc;
        currentDialogue = dialogue;

        currentLine = 0;
        dialogueBox.SetActive(true);
        currentState = State.Scrolling;

        currentDialogue.ResetProgress();

        nameText.gameObject.SetActive(currentDialogue.hasName);
        if (currentDialogue.systemType == DialogueSO.SystemType.NodeBased)
        {
            // Debug.Log("Node Based");
            splitDialogueLines = currentDialogue.nodes[currentDialogue.currentNodeIndex].content;
            CheckName();
            StartCoroutine(ProcessNodeSystem());
        }
        else
        {
            splitDialogueLines = currentDialogue.cachedLines; // 直接使用预处理数据
            CheckName();
            StartCoroutine(ProcessLegacySystem());
        }

    }
    #region Legacy System
    private IEnumerator ProcessLegacySystem()
    {
        eventMap = currentDialogue.events.ToDictionary(e => e.triggerLine, e => e.onReached);
        string triggerSymbol = "@trigger";
        if (eventMap != null && eventMap.TryGetValue(currentLine, out var evt))
        {
            splitDialogueLines[currentLine] = splitDialogueLines[currentLine].Replace(triggerSymbol, "");
            dialogueText.text = splitDialogueLines[currentLine];
            isScrolling = false;
            evt.Invoke();
        }

        yield return StartCoroutine(ScrollText(splitDialogueLines[currentLine]));
    }
    #endregion
    public void CheckName()
    {
        if (currentLine >= splitDialogueLines.Length) return;

        if (splitDialogueLines[currentLine].StartsWith("n-"))
        {
            nameText.text = splitDialogueLines[currentLine].Replace("n-", "");
            if (currentLine + 1 >= splitDialogueLines.Length)
            {
                EndDialogue();
                return;
            }
            currentLine++;
            if (currentLine >= splitDialogueLines.Length)
            {
                EndDialogue();
                return;
            }
        }
    }
    private void EndDialogue()
    {
        PlayerManager.instance.player.canMove = true;
        // if (npc.isShop) UI.instance.SwithToShop();
        if (currentDialogue.EndscriptableEffects != null)
        {
            foreach (var effect in currentDialogue.EndscriptableEffects)
            {
                if (effect != null)
                {
                    if (effect.ApplyTrigger())
                    {
                        // Debug.Log("Effect Applied");
                        effect.ApplyEffect(PlayerManager.instance.player.gameObject);

                    }

                }
            }
        }
        currentState = State.Inactive;
        dialogueBox.SetActive(false);
        choicePanel.SetActive(false);
        npc.isEntered = false;
        currentLine = 0; // 重置对话进度
    }

    #region Node Based System
    private IEnumerator ProcessNodeSystem()
    {
        var node = currentDialogue.nodes[currentDialogue.currentNodeIndex];

        // Debug.Log("Current Line: " + currentLine);
        if (node.triggerLine != -1 && node.triggerLine == currentLine)
        {
            //Debug.Log("Node Triggered"+ node.triggerLine);
            currentState = State.Choosing;
            ShowChoices(node.choices);
            splitDialogueLines[currentLine] = splitDialogueLines[currentLine].Replace(node.triggerSymbol, "");
            dialogueText.text = splitDialogueLines[currentLine];
            isScrolling = false;
            yield break;
        }
        yield return StartCoroutine(ScrollText(splitDialogueLines[currentLine]));

    }
    private void ShowChoices(Choice[] choices)
    {
       

        // 删除旧的选项
        foreach (var choice in currentChoices)
        {

            PoolMgr.Instance.Release(choice);
            // Destroy(choice);
        }
        currentChoices.Clear();

        List<Choice> validChoices = new List<Choice>();
        foreach (var choice in choices)
        {
            if (MeetsChoiceRequirements(choice))
            {
                validChoices.Add(choice);
            }
        }
        if (validChoices.Count == 0)
        {
            splitDialogueLines = currentDialogue.nodes[0].content;
            currentDialogue.currentNodeIndex = 0;
            currentLine++;
            currentState = State.Scrolling;
            CheckName();
            StartCoroutine(ProcessNodeSystem());
           // EndDialogue();
            return;
        }
        choicePanel.SetActive(true);
        // 生成新的选项按钮
        foreach (var choice in validChoices)
        {
            //// 检查选项条件
            //if (!MeetsChoiceRequirements(choice)) continue;

            GameObject button = PoolMgr.Instance.GetObj("ChoiceButton");
            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice));
            button.transform.SetParent(choicePanel.transform, false);
            // button.gameObject.name= choice.text;
            currentChoices.Add(button);
        }
        // 为按钮设置导航
        for (int i = 0; i < currentChoices.Count; i++)
        {
            var button = currentChoices[i].GetComponent<Button>();
            Navigation nav = button.navigation;
            nav.mode = Navigation.Mode.Explicit;

            // 设置上下导航
            if (i > 0)
                nav.selectOnUp = currentChoices[i - 1].GetComponent<Button>();
            if (i < currentChoices.Count - 1)
                nav.selectOnDown = currentChoices[i + 1].GetComponent<Button>();

            button.navigation = nav;
        }

        // 自动选中第一个按钮
        if (currentChoices.Count > 0)
        {
            StartCoroutine(SetFirstSelected());
            //EventSystem.current.SetSelectedGameObject(currentChoices[0]);
        }
    }
    IEnumerator SetFirstSelected()
    {
        yield return null; // 等待一帧确保UI渲染完成
        if (currentChoices.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(currentChoices[0]);
        }
    }
    private bool MeetsChoiceRequirements(Choice choice)
    {
        // 实现你的条件检查逻辑，例如：
        // if(Player.level < choice.minLevel) return false;
        // if(!string.IsNullOrEmpty(choice.requiredItem) && !Inventory.HasItem(choice.requiredItem)) return false;
        // 检查任务依赖
        if (choice.requireTaskCompletion && choice.taskToTrigger != null)
        {
            foreach (var prerequisite in choice.taskToTrigger.prerequisites)
            {
                if (prerequisite.status != TaskSO.TaskStatus.Completed)
                    return false;
            }
        }
        if (choice.taskToTrigger != null && choice.taskToTrigger.status != TaskSO.TaskStatus.NotStarted)
        {
            return false;
        }
        return true;
    }

    private void OnChoiceSelected(Choice choice)
    {
        choicePanel.SetActive(false);
        choice.onSelected?.Invoke();
        bool canApply = true;
        foreach (var effect in choice.scriptableEffects)
        {
            if (effect != null)
            {
                if (effect.ApplyTrigger())
                {
                    Debug.Log("Effect Applied");
                    effect.ApplyEffect(PlayerManager.instance.player.gameObject);

                }
                else
                {
                    canApply = false;
                }


            }
        }
        if (choice.taskToTrigger != null)
        {
            bool taskAccepted = TaskManager.Instance.TryAcceptTask(choice.taskToTrigger);
            if (!taskAccepted)
            {
                canApply = false;
                // 若任务接取失败，可在此处播放提示音效或显示UI
                Debug.Log("任务接取失败，请检查前置条件");
            }
        }
        // Debug.Log("Choice Selected: " + canApply);
        if (!canApply && choice.EndingDialogueIndex != 0)
        {
            currentLine = 0;
            splitDialogueLines = currentDialogue.nodes[choice.EndingDialogueIndex].content;
            currentDialogue.currentNodeIndex = choice.EndingDialogueIndex;
            currentState = State.Scrolling;
            CheckName();
            StartCoroutine(ProcessNodeSystem());
            return;
        }

        if (choice.nextDialogue != null)
        {
            StartDialogue(choice.nextDialogue, npc);
        }
        else
        {
            // Debug.Log(choice.nextDialogueIndex  +"   " + currentDialogue.nodes.Length);

            if (choice.nextDialogueIndex < currentDialogue.nodes.Length && choice.nextDialogueIndex > 0)
            {
                currentLine = 0;
                splitDialogueLines = currentDialogue.nodes[choice.nextDialogueIndex].content;
                currentDialogue.currentNodeIndex = choice.nextDialogueIndex;
            }
            else
            {
                splitDialogueLines = currentDialogue.nodes[0].content;
                currentDialogue.currentNodeIndex = 0;
                currentLine++;
                // Debug.Log("Current Line: " + currentLine);
            }

            currentState = State.Scrolling;
            CheckName();
            StartCoroutine(ProcessNodeSystem());
        }

    }
    #endregion
    private IEnumerator ScrollText(string content)
    {
        dialogueText.text = "";
        isScrolling = true;
        while (dialogueText.text.Length < content.Length && isScrolling)
        {
            dialogueText.text += content[dialogueText.text.Length];
            yield return new WaitForSeconds(scrollSpeed);
        }
        isScrolling = false;
    }
}
