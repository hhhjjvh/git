using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;



public class InputBindingManager1 : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionAsset inputActions;        // ���� Input Actions �ļ�
    public Transform bindingListParent;          // ���ڷ��ð���Ŀ�ĸ�����
    public GameObject bindingEntryPrefab;        // ����Ŀ��Ԥ����

    [Header("UI Components")]
    public TMP_Text currentDeviceText;           // ��ǰ�豸��ʾ
    public TMP_Text notificationText;            // ֪ͨ�ı�
    public Button resetToDefaultButton;          // �ָ�Ĭ�ϰ�ť
    public Button keyboardBindingButton;         // �л��������޸ĵİ�ť
    public Button gamepadBindingButton;          // �л����ֱ��޸ĵİ�ť

    private string currentBindingGroup = "Keyboard&Mouse"; // ��ǰ����
    private readonly string[] excludedActions = { "Move", "Look", "Fire" }; // �ų��Ĳ���

    private void Start()
    {
        // �������֧��
        EnableMouse();

        // ���ذ�����
        LoadBindings();

        // ��ʼ�����鲢���ɰ��б�
        UpdateBindingGroup(currentBindingGroup);
        //  GenerateBindingList();

        // ���ü��̺��ֱ���ť���¼�
        keyboardBindingButton.onClick.AddListener(SwitchToKeyboardBindings);
        gamepadBindingButton.onClick.AddListener(SwitchToGamepadBindings);
        resetToDefaultButton.onClick.AddListener(ResetToDefaultBindings);

        PrintAllBindings();
        // ���Զ������ڰ�������ʱ��ӡ��Ϣ
        inputActions.FindAction("Jump").performed += context => Debug.Log("jump action performed");
        inputActions.FindAction("Attack").performed += context => Debug.Log("attack action performed");
        //if(inputActions.)

        // InputManager.Instance.RestInput();

    }
    private void PrintAllBindings()
    {
        foreach (var actionMap in inputActions.actionMaps)
        {
            actionMap.Enable(); // �������� ActionMap
                                //  Debug.Log($"ActionMap: {actionMap.name}");
            foreach (var action in actionMap.actions)
            {
                // Debug.Log($"  Action: {action.name}");
                foreach (var binding in action.bindings)
                {

                    action.Enable();
                    //  Debug.Log($"    Binding: {binding.path} ({binding.effectivePath})");
                }
            }
        }
    }


    /// <summary>
    /// �л������̰���
    /// </summary>
    private void SwitchToKeyboardBindings()
    {
        currentBindingGroup = "Keyboard&Mouse";
        UpdateBindingGroup(currentBindingGroup);
        Debug.Log("Switched to Keyboard bindings.");
    }

    /// <summary>
    /// �л����ֱ�����
    /// </summary>
    private void SwitchToGamepadBindings()
    {
        currentBindingGroup = "Gamepad";
        UpdateBindingGroup(currentBindingGroup);
        Debug.Log("Switched to Gamepad bindings.");
    }

    /// <summary>
    /// ���°���
    /// </summary>
    private void UpdateBindingGroup(string newBindingGroup)
    {
        currentBindingGroup = newBindingGroup;
        currentDeviceText.text = $"Current Input: {currentBindingGroup}";

        foreach (var map in inputActions.actionMaps)
        {
            if (map.name == "Player")
            {
                map.bindingMask = InputBinding.MaskByGroup(currentBindingGroup); // ���ư���
            }
            else
            {
                map.bindingMask = null; // ȷ������ ActionMap ����Ӱ��
            }
        }

        RefreshBindingList();
    }

    /// <summary>
    /// ˢ�°��б�
    /// </summary>
    private void RefreshBindingList()
    {
        foreach (Transform child in bindingListParent)
        {
            //  Destroy(child.gameObject);
            PoolMgr.Instance.Release(child.gameObject);
        }

        GenerateBindingList();
    }

    /// <summary>
    /// ��̬���ɰ���Ŀ�б�
    /// </summary>
    private void GenerateBindingList()
    {
        var playerActionMap = inputActions.FindActionMap("Player");
        if (playerActionMap == null)
        {
            Debug.LogError("Action Map 'Player' not found!");
            return;
        }

        foreach (var action in playerActionMap.actions)
        {
            if (System.Array.Exists(excludedActions, excluded => excluded == action.name))
                continue;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];

                if (binding.isComposite || binding.isPartOfComposite)
                    continue;
                //Debug.Log($"Binding Groups: {binding.effectivePath}");
                string deviceType = GetDeviceTypeFromPath(binding.effectivePath);
                if (deviceType == null)
                    continue;

                if ((currentBindingGroup == "Keyboard&Mouse" && deviceType != "Keyboard" && deviceType != "Mouse") ||
                    (currentBindingGroup == "Gamepad" && deviceType != "Gamepad"))
                {
                    continue;
                }
                // Debug.Log($"    Binding: {binding.path} ({binding.effectivePath})");
                // Debug.Log($"Binding Groups: {binding.overridePath}");
                GameObject entry = PoolMgr.Instance.GetObj("BindingEntryPrefab");
                entry.transform.SetParent(bindingListParent, false);
                    //Instantiate(bindingEntryPrefab, bindingListParent);

                TMP_Text bindingNameText = entry.transform.Find("BindingName").GetComponent<TMP_Text>();
                bindingNameText.text = $"{playerActionMap.name}/{action.name}";

                TMP_Text bindingKeyText = entry.transform.Find("BindingKey").GetComponent<TMP_Text>();
                bindingKeyText.text = GetBindingDisplayName(action, i);

                Button rebindButton = entry.transform.Find("RebindButton").GetComponent<Button>();
                int bindingIndex = i;
                rebindButton.onClick.RemoveAllListeners();
                rebindButton.onClick.AddListener(() =>// RebindAction(action.name));
                StartRebinding(action, bindingIndex, bindingKeyText));
            }
        }
    }

    /// <summary>
    /// ��ȡ��·�����豸����
    /// </summary>
    private string GetDeviceTypeFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        if (path.Contains("<Keyboard>"))
            return "Keyboard";
        if (path.Contains("<Mouse>"))
            return "Mouse";
        if (path.Contains("<Gamepad>"))
            return "Gamepad";

        return null;
    }

    /// <summary>
    /// ��ȡ�����󶨵���ʾ����
    /// </summary>
    private string GetBindingDisplayName(InputAction action, int bindingIndex)
    {
        return InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    /// <summary>
    /// ��ʼ���°󶨰���
    /// </summary>
    private void StartRebinding(InputAction action, int bindingIndex, TMP_Text bindingKeyText)
    {
        notificationText.text = "Press a key to rebind...";

        string oldControlPath = action.bindings[bindingIndex].effectivePath;
        // ��ʼ���°�
        // var rebindingOperation = action.PerformInteractiveRebinding(bindingIndex);
        action.Disable(); // ���õ�ǰ��������ֹ�����°󶨹����д���
        // rebindingOperation
        action.PerformInteractiveRebinding(bindingIndex)
        //  .WithBindingGroup(currentBindingGroup) // ���Ƶ���ǰ����
          .WithControlsExcluding("Mouse")
          .OnMatchWaitForAnother(.1f)
          .OnComplete(operation =>
          {

              string newControlPath = operation.action.bindings[bindingIndex].effectivePath;


              //if (!newControlPath.StartsWith("<"))
              //{
              //    if (newControlPath.StartsWith("/Keyboard"))
              //    {
              //        string key = newControlPath.Replace("/Keyboard", "");
              //        newControlPath = $"<Keyboard>{key}";
              //    }
              //    else if (newControlPath.StartsWith("/Mouse"))
              //    {
              //        string mouseButton = newControlPath.Replace("/Mouse", "");
              //        newControlPath = $"<Mouse>{mouseButton}";
              //    }
              //    else if (newControlPath.StartsWith("/Gamepad"))
              //    {
              //        string gamepadButton = newControlPath.Replace("/Gamepad", "");
              //        newControlPath = $"<Gamepad>{gamepadButton}";
              //    }
              //}
              Debug.Log($"New control path: {newControlPath}");

              // �������·���Ƿ��ǺϷ��ļ��̰�������ĸ�����֣�
              if ((currentBindingGroup == "Keyboard&Mouse" && !IsValidKey(newControlPath)) || currentBindingGroup == "Gamepad" && !newControlPath.Contains("<Gamepad>"))
              {
                  notificationText.text = "Invalid input! Only letters or numbers are allowed.";
                  Debug.Log($"Invalid keyboard input: {newControlPath}");
                  action.Enable(); // �������õ�ǰ����
                  operation.Dispose();
                  return; // ֹͣ�󶨲���
              }

              // ����ͻ������������
              if (IsBindingInUse(newControlPath, action))
              {
                  SwapBindings(newControlPath, action, bindingIndex, oldControlPath);
                  notificationText.text = "Key swapped due to conflict!";
              }
              else
              {

                  action.ApplyBindingOverride(bindingIndex, newControlPath);

              }

              bindingKeyText.text = GetBindingDisplayName(action, bindingIndex);
              notificationText.text = "Rebinding successful!";

              SaveBindings();

              UpdateBindingGroup(currentBindingGroup);
              action.Enable(); // �������õ�ǰ����
              operation.Dispose();
          })
          .Start();
    }


    /// <summary>
    /// ��֤�����Ƿ��ǺϷ�����ĸ�����ּ�
    /// </summary>
    private bool IsValidKey(string controlPath)
    {
        if (controlPath.StartsWith("<Keyboard>/"))
        {
            string key = controlPath.Replace("<Keyboard>/", ""); // ��ȡ��������

            // �ж��Ƿ�Ϊ���ַ���ĸ������
            return key.Length == 1 && char.IsLetterOrDigit(key[0]);
        }
        return false; // �Ǽ��̰�����Ƿ�����
    }

    /// <summary>
    /// ����Ƿ�����������ռ�ô�·��
    /// </summary>
    private bool IsBindingInUse(string controlPath, InputAction currentAction)
    {
        var playerActionMap = inputActions.FindActionMap("Player");
        if (playerActionMap == null) return false;

        foreach (var action in playerActionMap.actions)
        {
            if (action == currentAction) continue;

            foreach (var binding in action.bindings)
            {
                if (binding.effectivePath == controlPath)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// ������ͻ�İ�
    /// </summary>
    //private void SwapBindings(string controlPath, InputAction currentAction, int currentBindingIndex)
    //{
    //    var playerActionMap = inputActions.FindActionMap("Player");
    //    if (playerActionMap == null) return;

    //    foreach (var action in playerActionMap.actions)
    //    {
    //        foreach (var binding in action.bindings)
    //        {
    //            if (binding.effectivePath == controlPath)
    //            {
    //                Debug.Log($"Swapping bindings: {currentAction.name} <-> {action.name}");

    //                string originalPath = currentAction.bindings[currentBindingIndex].effectivePath;

    //                action.ApplyBindingOverride(binding.path, originalPath);
    //                currentAction.ApplyBindingOverride(currentBindingIndex, controlPath);

    //                return;
    //            }
    //        }
    //    }
    //}


    private void SwapBindings(string controlPath, InputAction currentAction, int currentBindingIndex, string oldPath)
    {
        // ��֤�������Ƿ���Ч
        if (currentBindingIndex < 0 || currentBindingIndex >= currentAction.bindings.Count)
        {
            Debug.LogError("Invalid binding index!");
            return;
        }

        var playerActionMap = inputActions.FindActionMap("Player");
        if (playerActionMap == null) return;

        foreach (var action in playerActionMap.actions)
        {
            foreach (var binding in action.bindings)
            {
                //�ų�����
                if (action == currentAction) continue;


                if (binding.effectivePath == controlPath)
                {
                    Debug.Log($"Swapping bindings: {currentAction.name} <-> {action.name}");

                    // ��ȡ��ǰ�󶨵���Ч·��
                    //string originalPath = currentAction.bindings[currentBindingIndex].effectivePath;

                    // ������·��
                    currentAction.ApplyBindingOverride(currentBindingIndex, controlPath);
                    Debug.Log($"New binding for {currentAction.name}: {controlPath}");
                    action.ApplyBindingOverride(currentBindingIndex, oldPath);
                    Debug.Log($"New binding for {action.name}: {currentBindingIndex + oldPath}");


                    return;
                }
            }
        }

        Debug.LogWarning($"No conflicting binding found for path: {controlPath}");
    }



    /// <summary>
    /// ����󶨵��־û��洢
    /// </summary>

    private void SaveBindings()
    {

        try
        {
            string bindings = inputActions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("Bindings", bindings);
            PlayerPrefs.Save();
            Debug.Log("Bindings saved successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save bindings: {ex.Message}");
        }
        InputManager.Instance.RestInput();
    }

    /// <summary>
    /// �� PlayerPrefs ���ذ�
    /// </summary>
    private void LoadBindings()
    {

        if (PlayerPrefs.HasKey("Bindings"))
        {
            try
            {
                string bindings = PlayerPrefs.GetString("Bindings");
                inputActions.LoadBindingOverridesFromJson(bindings);
                Debug.Log("Bindings loaded successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load bindings: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("No bindings found to load.");
        }
        InputManager.Instance.RestInput();
    }



    /// <summary>
    /// �ָ������󶨵�Ĭ��ֵ
    /// </summary>
    private void ResetToDefaultBindings()
    {
        var playerActionMap = inputActions.FindActionMap("Player");
        if (playerActionMap == null)
        {
            Debug.LogError("Action Map 'Player' not found!");
            return;
        }

        foreach (var action in playerActionMap.actions)
        {
            action.RemoveAllBindingOverrides();
        }

        PlayerPrefs.DeleteKey("Bindings");
        //  LoadBindings();
        RefreshBindingList();
        notificationText.text = "All bindings reset to default.";
        InputManager.Instance.RestInput();
    }

    /// <summary>


    /// <summary>
    /// �����������
    /// </summary>
    private void EnableMouse()
    {
        var mouse = Mouse.current;
        if (mouse != null && !mouse.enabled)
        {
            InputSystem.EnableDevice(mouse);
            Debug.Log("Mouse input enabled.");
        }
    }
}

