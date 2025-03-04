using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;



public class InputBindingManager1 : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionAsset inputActions;        // 引用 Input Actions 文件
    public Transform bindingListParent;          // 用于放置绑定条目的父对象
    public GameObject bindingEntryPrefab;        // 绑定条目的预制体

    [Header("UI Components")]
    public TMP_Text currentDeviceText;           // 当前设备显示
    public TMP_Text notificationText;            // 通知文本
    public Button resetToDefaultButton;          // 恢复默认按钮
    public Button keyboardBindingButton;         // 切换到键盘修改的按钮
    public Button gamepadBindingButton;          // 切换到手柄修改的按钮

    private string currentBindingGroup = "Keyboard&Mouse"; // 当前绑定组
    private readonly string[] excludedActions = { "Move", "Look", "Fire" }; // 排除的操作

    private void Start()
    {
        // 启用鼠标支持
        EnableMouse();

        // 加载绑定配置
        LoadBindings();

        // 初始化绑定组并生成绑定列表
        UpdateBindingGroup(currentBindingGroup);
        //  GenerateBindingList();

        // 设置键盘和手柄按钮的事件
        keyboardBindingButton.onClick.AddListener(SwitchToKeyboardBindings);
        gamepadBindingButton.onClick.AddListener(SwitchToGamepadBindings);
        resetToDefaultButton.onClick.AddListener(ResetToDefaultBindings);

        PrintAllBindings();
        // 测试动作：在按键触发时打印消息
        inputActions.FindAction("Jump").performed += context => Debug.Log("jump action performed");
        inputActions.FindAction("Attack").performed += context => Debug.Log("attack action performed");
        //if(inputActions.)

        // InputManager.Instance.RestInput();

    }
    private void PrintAllBindings()
    {
        foreach (var actionMap in inputActions.actionMaps)
        {
            actionMap.Enable(); // 启用整个 ActionMap
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
    /// 切换到键盘绑定组
    /// </summary>
    private void SwitchToKeyboardBindings()
    {
        currentBindingGroup = "Keyboard&Mouse";
        UpdateBindingGroup(currentBindingGroup);
        Debug.Log("Switched to Keyboard bindings.");
    }

    /// <summary>
    /// 切换到手柄绑定组
    /// </summary>
    private void SwitchToGamepadBindings()
    {
        currentBindingGroup = "Gamepad";
        UpdateBindingGroup(currentBindingGroup);
        Debug.Log("Switched to Gamepad bindings.");
    }

    /// <summary>
    /// 更新绑定组
    /// </summary>
    private void UpdateBindingGroup(string newBindingGroup)
    {
        currentBindingGroup = newBindingGroup;
        currentDeviceText.text = $"Current Input: {currentBindingGroup}";

        foreach (var map in inputActions.actionMaps)
        {
            if (map.name == "Player")
            {
                map.bindingMask = InputBinding.MaskByGroup(currentBindingGroup); // 限制绑定组
            }
            else
            {
                map.bindingMask = null; // 确保其他 ActionMap 不受影响
            }
        }

        RefreshBindingList();
    }

    /// <summary>
    /// 刷新绑定列表
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
    /// 动态生成绑定条目列表
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
    /// 获取绑定路径的设备类型
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
    /// 获取按键绑定的显示名称
    /// </summary>
    private string GetBindingDisplayName(InputAction action, int bindingIndex)
    {
        return InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    /// <summary>
    /// 开始重新绑定按键
    /// </summary>
    private void StartRebinding(InputAction action, int bindingIndex, TMP_Text bindingKeyText)
    {
        notificationText.text = "Press a key to rebind...";

        string oldControlPath = action.bindings[bindingIndex].effectivePath;
        // 开始重新绑定
        // var rebindingOperation = action.PerformInteractiveRebinding(bindingIndex);
        action.Disable(); // 禁用当前操作，防止在重新绑定过程中触发
        // rebindingOperation
        action.PerformInteractiveRebinding(bindingIndex)
        //  .WithBindingGroup(currentBindingGroup) // 限制到当前绑定组
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

              // 检查输入路径是否是合法的键盘按键（字母或数字）
              if ((currentBindingGroup == "Keyboard&Mouse" && !IsValidKey(newControlPath)) || currentBindingGroup == "Gamepad" && !newControlPath.Contains("<Gamepad>"))
              {
                  notificationText.text = "Invalid input! Only letters or numbers are allowed.";
                  Debug.Log($"Invalid keyboard input: {newControlPath}");
                  action.Enable(); // 重新启用当前操作
                  operation.Dispose();
                  return; // 停止绑定操作
              }

              // 检查冲突并交换按键绑定
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
              action.Enable(); // 重新启用当前操作
              operation.Dispose();
          })
          .Start();
    }


    /// <summary>
    /// 验证输入是否是合法的字母或数字键
    /// </summary>
    private bool IsValidKey(string controlPath)
    {
        if (controlPath.StartsWith("<Keyboard>/"))
        {
            string key = controlPath.Replace("<Keyboard>/", ""); // 提取按键名称

            // 判断是否为单字符字母或数字
            return key.Length == 1 && char.IsLetterOrDigit(key[0]);
        }
        return false; // 非键盘按键或非法输入
    }

    /// <summary>
    /// 检查是否已有其他绑定占用此路径
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
    /// 交换冲突的绑定
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
        // 验证绑定索引是否有效
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
                //排除自身
                if (action == currentAction) continue;


                if (binding.effectivePath == controlPath)
                {
                    Debug.Log($"Swapping bindings: {currentAction.name} <-> {action.name}");

                    // 获取当前绑定的有效路径
                    //string originalPath = currentAction.bindings[currentBindingIndex].effectivePath;

                    // 交换绑定路径
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
    /// 保存绑定到持久化存储
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
    /// 从 PlayerPrefs 加载绑定
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
    /// 恢复按键绑定到默认值
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
    /// 启用鼠标输入
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

