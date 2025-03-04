using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Linq; // 如果使用的是TextMeshPro


// 定义按键记录结构
public struct KeyPressEntry
{
    public string actionName;
    public float pressTime;

    public KeyPressEntry(string name, float time)
    {
        actionName = name;
        pressTime = time;
    }
}

public class InputManager : MonoBehaviour
{
    // public InputActionAsset playerInput;
    public PlayerInput playerInput;
    public static InputManager Instance;

    public Vector2 moveInput;

    // private Dictionary<string, float> keyPressTimes = new Dictionary<string, float>(); // 记录按键按下时间
    private List<KeyPressEntry> keyPressEntries = new List<KeyPressEntry>();
    private float comboTimeWindow = 1.2f; // 允许松开按键后维持的时间（秒）
    public TextMeshProUGUI keyStatusText; // 如果使用 TextMeshPro
   // private float continuousKeyWindow = 0.2f; // 连续按键时间窗口，控制连续按下时的间隔
    private bool lastUp, lastDown, lastLeft, lastRight;
    private string keyStatus = "按键状态: ";
    public bool canJump { get; set; }
    public bool canAttack { get; set; }
    public bool canDash { get; set; }
    public bool canInteract { get; set; }
    public bool canUseItem { get; set; }
    public bool canUseSkill { get; set; }
    public bool canDistanceAttack { get; set; }
    public bool canParry { get; set; }
    public bool canPause { get; set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }



        playerInput = new PlayerInput();





    }
    void OnEnable()
    {
        playerInput.Enable();
        LoadBindings();
        
        playerInput.Player.Jump.started+= OnJumpStarted;
        playerInput.Player.Jump.canceled += OnJumpCanceled;
        playerInput.Player.Attack.started+= OnAttackStarted;
        playerInput.Player.Attack.canceled += OnAttackCanceled;
        playerInput.Player.Dash.started+= OnDashStarted;
        playerInput.Player.Dash.canceled += OnDashCanceled;
        playerInput.Player.Interach.started+= OnInteractStarted;
        playerInput.Player.Interach.canceled += OnInteractCanceled;
        playerInput.Player.UsFlask.started+= OnUseItemStarted;
        playerInput.Player.UsFlask.canceled += OnUseItemCanceled;
        playerInput.Player.UsSkill.started+= OnUseSkillStarted;
        playerInput.Player.UsSkill.canceled += OnUseSkillCanceled;
        playerInput.Player.DistantAttack.started+= OnDistanceAttackStarted;
        playerInput.Player.DistantAttack.canceled += OnDistanceAttackCanceled;
        playerInput.Player.Parry.started+= OnParryStarted;
        playerInput.Player.Parry.canceled += OnParryCanceled;
        playerInput.Player.Pause.started+= OnPauseStarted;
        playerInput.Player.Pause.canceled += OnPauseCanceled;

        // 注册所有按键
        RegisterKeyWithEvents("Attack", playerInput.Player.Attack);
        RegisterKeyWithEvents("UsSkill", playerInput.Player.UsSkill);
        RegisterKeyWithEvents("DistantAttack", playerInput.Player.DistantAttack);
        RegisterKeyWithEvents("Dash", playerInput.Player.Dash);
        RegisterKeyWithEvents("Jump", playerInput.Player.Jump);

        //RegisterKeyWithEvents("Move", playerInput.Player.Move); // 移动作为整体
    }

    private void OnJumpStarted(InputAction.CallbackContext context)=>canJump = true;
    private void OnJumpCanceled(InputAction.CallbackContext context)=>canJump = false;
    private void OnAttackStarted(InputAction.CallbackContext context)=>canAttack = true;
    private void OnAttackCanceled(InputAction.CallbackContext context)=>canAttack = false;
    private void OnDashStarted(InputAction.CallbackContext context)=>canDash = true;
    private void OnDashCanceled(InputAction.CallbackContext context)=>canDash = false;
    private void OnInteractStarted(InputAction.CallbackContext context)=>canInteract = true;
    private void OnInteractCanceled(InputAction.CallbackContext context)=>canInteract = false;
    private void OnUseItemStarted(InputAction.CallbackContext context)=>canUseItem = true;
    private void OnUseItemCanceled(InputAction.CallbackContext context)=>canUseItem = false;
    private void OnUseSkillStarted(InputAction.CallbackContext context)=>canUseSkill = true;
    private void OnUseSkillCanceled(InputAction.CallbackContext context)=>canUseSkill = false;
    private void OnDistanceAttackStarted(InputAction.CallbackContext context)=>canDistanceAttack = true;
    private void OnDistanceAttackCanceled(InputAction.CallbackContext context)=>canDistanceAttack = false;
    private void OnParryStarted(InputAction.CallbackContext context)=>canParry = true;
    private void OnParryCanceled(InputAction.CallbackContext context)=>canParry = false;
    private void OnPauseStarted(InputAction.CallbackContext context)=>canPause = true;
    private void OnPauseCanceled(InputAction.CallbackContext context)=>canPause = false;

    void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Jump.started -= OnJumpStarted;
        playerInput.Player.Jump.canceled -= OnJumpCanceled;
        playerInput.Player.Attack.started -= OnAttackStarted;
        playerInput.Player.Attack.canceled -= OnAttackCanceled;
        playerInput.Player.Dash.started -= OnDashStarted;
        playerInput.Player.Dash.canceled -= OnDashCanceled;
        playerInput.Player.Interach.started -= OnInteractStarted;
        playerInput.Player.Interach.canceled -= OnInteractCanceled;
        playerInput.Player.UsFlask.started -= OnUseItemStarted;
        playerInput.Player.UsFlask.canceled -= OnUseItemCanceled;
        playerInput.Player.UsSkill.started -= OnUseSkillStarted;
        playerInput.Player.UsSkill.canceled -= OnUseSkillCanceled;
        playerInput.Player.DistantAttack.started -= OnDistanceAttackStarted;
        playerInput.Player.DistantAttack.canceled -= OnDistanceAttackCanceled;
        playerInput.Player.Parry.started -= OnParryStarted;
        playerInput.Player.Parry.canceled -= OnParryCanceled;
        playerInput.Player.Pause.started -= OnPauseStarted;
        playerInput.Player.Pause.canceled -= OnPauseCanceled;

        RemoveKeyWithEvents("Jump", playerInput.Player.Jump);
        RemoveKeyWithEvents("Attack", playerInput.Player.Attack);
        RemoveKeyWithEvents("Dash", playerInput.Player.Dash);
        RemoveKeyWithEvents("UseSkill", playerInput.Player.UsSkill);
        RemoveKeyWithEvents("DistantAttack", playerInput.Player.DistantAttack);
    }

    public void RestInput()
    {     
        LoadBindings();     
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
        //// 只在状态变化时注册
        //if (moveInput.y > 0.5f && !lastUp) RegisterKey("Move_Up");
        //if (moveInput.y < -0.5f && !lastDown) RegisterKey("Move_Down");
        //if (moveInput.x < -0.5f && !lastLeft) RegisterKey("Move_Left");
        //if (moveInput.x > 0.5f && !lastRight) RegisterKey("Move_Right");

        //// 更新状态
        //lastUp = moveInput.y > 0.5f;
        //lastDown = moveInput.y < -0.5f;
        //lastLeft = moveInput.x < -0.5f;
        //lastRight = moveInput.x > 0.5f;
        if(moveInput.y > 0.5f!=lastUp)
        {
            if (moveInput.y > 0.5f) RegisterKey("Move_Up");
           // else UnregisterKey("Move_Up");
            lastUp = moveInput.y > 0.5f;
        }
        if (moveInput.y < -0.5f != lastDown)
        {
            if (moveInput.y < -0.5f) RegisterKey("Move_Down");
           // else UnregisterKey("Move_Down");
            lastDown = moveInput.y < -0.5f;
        }
        if (moveInput.x < -0.5f != lastLeft)
        {
            if (moveInput.x < -0.5f) RegisterKey("Move_Left");
            //else UnregisterKey("Move_Left");
            lastLeft = moveInput.x < -0.5f;
        }
        if (moveInput.x > 0.5f != lastRight)
        {
            if (moveInput.x > 0.5f) RegisterKey("Move_Right");
            //else UnregisterKey("Move_Right");
            lastRight = moveInput.x > 0.5f;
        }
        // 处理超时按键
        RemoveExpiredKeys();
        keyStatus = "按键状态: " + string.Join(", ", GetPressedKeys());
        keyStatusText.text = keyStatus; // 更新UI文本
    }
    private void RegisterKeyWithEvents(string actionName, InputAction action)
    {
        action.started += ctx => RegisterKey(actionName);
        //action.canceled += ctx => UnregisterKey(actionName);
    }
    private void RemoveKeyWithEvents(string actionName, InputAction action)
    {
        action.started -= ctx => RegisterKey(actionName);
    }

    // 注册按键
    private void RegisterKey(string actionName)
    {
        keyPressEntries.Add(new KeyPressEntry(actionName, Time.time));
        ComboSystem.RegisterKey(actionName);  // 注册按键
    }
    // 取消注册按键
    
    private void RemoveExpiredKeys()
    {
        keyPressEntries.RemoveAll(entry => Time.time - entry.pressTime > comboTimeWindow);
    }
    // 获取当前按下的按键
    public List<string> GetPressedKeys()
    {     
        return keyPressEntries.Where(entry => Time.time - entry.pressTime <= comboTimeWindow)
                              .Select(entry => entry.actionName)
                              .ToList();
    }
    public void RestKeyPressEntries()
    {
        keyPressEntries.Clear();
    }
    private void SaveBindings()
    {
        //InputManager.Instance.RestInput();
        try
        {
            string bindings = playerInput.asset.SaveBindingOverridesAsJson();
            //inputActions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("Bindings", bindings);
            PlayerPrefs.Save();
            // Debug.Log("Bindings saved successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save bindings: {ex.Message}");
        }
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
                playerInput.asset.LoadBindingOverridesFromJson(bindings);
                // Debug.Log("Bindings loaded successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load bindings: {ex.Message}");
            }
        }
        else
        {
            foreach (var action in playerInput.asset)
            {
                action.RemoveAllBindingOverrides();
            }
        }
    }
}
