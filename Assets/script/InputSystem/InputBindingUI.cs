using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class InputBindingManager : MonoBehaviour
{
    [Header("��������")]
    public InputActionAsset inputActions;
    public Transform bindingListParent;
    public GameObject bindingEntryPrefab;

    [Header("UI���")]
    public TMP_Text currentDeviceText;
    public TMP_Text notificationText;
    public Button resetToDefaultButton;
    public Button keyboardBindingButton;
    public Button gamepadBindingButton;

    private string currentBindingGroup = "Keyboard&Mouse";
    private readonly string[] excludedActions = { "Move", "Look", "Fire" };

    private void Start()
    {
        EnableMouse();
        LoadBindings();
        UpdateBindingGroup(currentBindingGroup);
        keyboardBindingButton.onClick.AddListener(SwitchToKeyboardBindings);
        gamepadBindingButton.onClick.AddListener(SwitchToGamepadBindings);
        resetToDefaultButton.onClick.AddListener(ResetToDefaultBindings);
    }

    private void SwitchToKeyboardBindings()
    {
        currentBindingGroup = "Keyboard&Mouse";
        UpdateBindingGroup(currentBindingGroup);
    }

    private void SwitchToGamepadBindings()
    {
        currentBindingGroup = "Gamepad";
        UpdateBindingGroup(currentBindingGroup);
    }

    private void UpdateBindingGroup(string newBindingGroup)
    {
        currentBindingGroup = newBindingGroup;
        currentDeviceText.text = $"��ǰ�����豸: {currentBindingGroup}";
        foreach (var map in inputActions.actionMaps)
        {
            map.bindingMask = map.name == "Player" ? InputBinding.MaskByGroup(currentBindingGroup) : null;
        }
        RefreshBindingList();
    }

    private void RefreshBindingList()
    {
        foreach (Transform child in bindingListParent)
        {
            Destroy(child.gameObject);
        }
        GenerateBindingList();
    }

    private void GenerateBindingList()
    {
        var playerActionMap = inputActions.FindActionMap("Player");
        if (playerActionMap == null)
        {
            notificationText.text = "δ�ҵ�����ӳ�� 'Player'";
            return;
        }

        foreach (var action in playerActionMap.actions)
        {
            if (System.Array.Exists(excludedActions, excluded => excluded == action.name)) continue;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];
                if (binding.isComposite || binding.isPartOfComposite) continue;

                string deviceType = GetDeviceTypeFromPath(binding.effectivePath);
                if (deviceType == null) continue;

                if ((currentBindingGroup == "Keyboard&Mouse" && deviceType != "Keyboard" && deviceType != "Mouse") ||
                    (currentBindingGroup == "Gamepad" && deviceType != "Gamepad"))
                {
                    continue;
                }

                GameObject entry = Instantiate(bindingEntryPrefab, bindingListParent);

                TMP_Text bindingNameText = entry.transform.Find("BindingName").GetComponent<TMP_Text>();
                bindingNameText.text =GetActionname(action.name);
                    // $"{playerActionMap.name}/{action.name}";

                Button rebindButton = entry.transform.Find("RebindButton").GetComponent<Button>();

                TMP_Text bindingKeyText = rebindButton.transform.Find("BindingKey").GetComponent<TMP_Text>();
                bindingKeyText.text = GetBindingDisplayName(action, i);
                int bindingIndex = i;
                rebindButton.onClick.AddListener(() => StartRebinding(action, bindingIndex, bindingKeyText));
            }
        }
    }
    private string GetActionname(string path)
    {
       
        switch (path)
        {
            case "Jump": return "��Ծ";
            case "Attack": return "����";
            case "Dash": return "���";
            case "Interach": return "����";
            case "UsFlask": return "ʹ��ҩˮ";
            case "UsSkill": return "ʹ�ü���";
            case "DistantAttack": return "Զ�̹���";
            case "Parry": return "��";
            case "Pause": return "��ͣ";
            default: return path;
        }
    }
    private string GetDeviceTypeFromPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (path.Contains("<Keyboard>")) return "Keyboard";
        if (path.Contains("<Mouse>")) return "Mouse";
        if (path.Contains("<Gamepad>")) return "Gamepad";
        return null;
    }

    private string GetBindingDisplayName(InputAction action, int bindingIndex)
    {
        return InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private void StartRebinding(InputAction action, int bindingIndex, TMP_Text bindingKeyText)
    {
        notificationText.text = "���¼������°�...";
        string oldControlPath = action.bindings[bindingIndex].effectivePath;
        action.Disable();

        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(.1f)
            .OnComplete(operation =>
            {
                string newControlPath = operation.action.bindings[bindingIndex].effectivePath;

                if ((currentBindingGroup == "Keyboard&Mouse" && !IsValidKey(newControlPath)) || currentBindingGroup == "Gamepad" && !newControlPath.Contains("<Gamepad>"))
                {
                    notificationText.text = "��Ч���룡";
                    action.Enable();
                    operation.Dispose();
                    action.ApplyBindingOverride(bindingIndex, oldControlPath);
                    AudioManager.instance.PlaySFX(17, null);
                    return;
                }

                if (IsBindingInUse(newControlPath, action))
                {
                    SwapBindings(newControlPath, action, bindingIndex, oldControlPath);
                    notificationText.text = "�������ͻ����������";
                }
                else
                {
                    action.ApplyBindingOverride(bindingIndex, newControlPath);
                    notificationText.text = "�����󶨳ɹ���";
                }
                AudioManager.instance.PlaySFX(20, null);
                bindingKeyText.text = GetBindingDisplayName(action, bindingIndex);
                //notificationText.text = "�����󶨳ɹ���";

                SaveBindings();
                UpdateBindingGroup(currentBindingGroup);
                action.Enable();
                operation.Dispose();
            })
            .Start();
    }

    private bool IsValidKey(string controlPath)
    {
        if (controlPath.StartsWith("<Keyboard>/"))
        {
            string key = controlPath.Replace("<Keyboard>/", "");
            return key.Length == 1 && char.IsLetterOrDigit(key[0]);
        }
        return false;
    }

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

    private void SwapBindings(string controlPath, InputAction currentAction, int currentBindingIndex, string oldPath)
    {
        if (currentBindingIndex < 0 || currentBindingIndex >= currentAction.bindings.Count) return;

        var playerActionMap = inputActions.FindActionMap("Player");
        if (playerActionMap == null) return;

        foreach (var action in playerActionMap.actions)
        {
            foreach (var binding in action.bindings)
            {
                if (action == currentAction) continue;

                if (binding.effectivePath == controlPath)
                {
                    currentAction.ApplyBindingOverride(currentBindingIndex, controlPath);
                    action.ApplyBindingOverride(currentBindingIndex, oldPath);
                    return;
                }
            }
        }
    }

    private void SaveBindings()
    {
        string bindings = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("Bindings", bindings);
        PlayerPrefs.Save();
        InputManager.Instance.RestInput();
    }

    private void LoadBindings()
    {
        if (PlayerPrefs.HasKey("Bindings"))
        {
            string bindings = PlayerPrefs.GetString("Bindings");
            inputActions.LoadBindingOverridesFromJson(bindings);
        }
        InputManager.Instance.RestInput();
    }

    private void ResetToDefaultBindings()
    {
        var playerActionMap = inputActions.FindActionMap("Player");
        if (playerActionMap == null) return;

        foreach (var action in playerActionMap.actions)
        {
            action.RemoveAllBindingOverrides();
        }

        PlayerPrefs.DeleteKey("Bindings");
        RefreshBindingList();
        notificationText.text = "�������ѻָ�Ĭ��ֵ��";
        InputManager.Instance.RestInput();
    }

    private void EnableMouse()
    {
        var mouse = Mouse.current;
        if (mouse != null && !mouse.enabled)
        {
            InputSystem.EnableDevice(mouse);
        }
    }
}
