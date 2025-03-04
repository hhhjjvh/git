using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Linq; // ���ʹ�õ���TextMeshPro


// ���尴����¼�ṹ
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

    // private Dictionary<string, float> keyPressTimes = new Dictionary<string, float>(); // ��¼��������ʱ��
    private List<KeyPressEntry> keyPressEntries = new List<KeyPressEntry>();
    private float comboTimeWindow = 1.2f; // �����ɿ�������ά�ֵ�ʱ�䣨�룩
    public TextMeshProUGUI keyStatusText; // ���ʹ�� TextMeshPro
   // private float continuousKeyWindow = 0.2f; // ��������ʱ�䴰�ڣ�������������ʱ�ļ��
    private bool lastUp, lastDown, lastLeft, lastRight;
    private string keyStatus = "����״̬: ";
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

        // ע�����а���
        RegisterKeyWithEvents("Attack", playerInput.Player.Attack);
        RegisterKeyWithEvents("UsSkill", playerInput.Player.UsSkill);
        RegisterKeyWithEvents("DistantAttack", playerInput.Player.DistantAttack);
        RegisterKeyWithEvents("Dash", playerInput.Player.Dash);
        RegisterKeyWithEvents("Jump", playerInput.Player.Jump);

        //RegisterKeyWithEvents("Move", playerInput.Player.Move); // �ƶ���Ϊ����
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
        //// ֻ��״̬�仯ʱע��
        //if (moveInput.y > 0.5f && !lastUp) RegisterKey("Move_Up");
        //if (moveInput.y < -0.5f && !lastDown) RegisterKey("Move_Down");
        //if (moveInput.x < -0.5f && !lastLeft) RegisterKey("Move_Left");
        //if (moveInput.x > 0.5f && !lastRight) RegisterKey("Move_Right");

        //// ����״̬
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
        // ����ʱ����
        RemoveExpiredKeys();
        keyStatus = "����״̬: " + string.Join(", ", GetPressedKeys());
        keyStatusText.text = keyStatus; // ����UI�ı�
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

    // ע�ᰴ��
    private void RegisterKey(string actionName)
    {
        keyPressEntries.Add(new KeyPressEntry(actionName, Time.time));
        ComboSystem.RegisterKey(actionName);  // ע�ᰴ��
    }
    // ȡ��ע�ᰴ��
    
    private void RemoveExpiredKeys()
    {
        keyPressEntries.RemoveAll(entry => Time.time - entry.pressTime > comboTimeWindow);
    }
    // ��ȡ��ǰ���µİ���
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
    /// �� PlayerPrefs ���ذ�
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
