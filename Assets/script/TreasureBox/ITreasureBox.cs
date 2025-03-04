using System.Collections.Generic;
using UnityEngine;

public enum TreasureBoxType
{
    WhiteTreasureBox,
    BlueTreasureBox,
    BrownTreasureBox,
    GreenTreasureBox,
    GoldTreasureBox
}


public abstract class ITreasureBox : MonoBehaviour,IInteractable
{
   protected TreasureBoxType treasureBoxType;
    protected Animator m_Animator;
    protected SpriteRenderer m_SpriteRenderer;
    public Sprite icon;

    //protected player1 player = PlayerManager.instance.player;
    protected GameObject Name;
    protected bool isPlayerEnter;
    private AnimatorStateInfo info;
    protected bool isFinish;
    protected List<ItemData> dropItems = new List<ItemData>();
    public void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        icon = m_SpriteRenderer.sprite;

    }
    protected virtual void Start()
    {
        //player =
        Name = transform.Find("Name")?.gameObject;
        m_Animator = GetComponent<Animator>();
        
    }
    protected virtual void OnEnable()
    {
        Name = transform.Find("Name")?.gameObject;
        m_Animator = GetComponent<Animator>();
        isFinish = false;
        isPlayerEnter = false;
       
        //重置动画
        // m_Animator.Play("Open");
        m_Animator.Play("Open", -1, 0f);
        m_Animator.enabled = false;
    }
    public void ResetAnimation()
    {
        
    }

    protected virtual void OnDisable()
    {
        m_Animator.Play("Open", -1, 0f);  // 将动画重置到第一帧
        m_Animator.enabled = false;
        //动画第一帧
        //m_SpriteRenderer.sprite = icon;
    }
    protected virtual void Update()
    {
        if (!isFinish)
        {
            if (isPlayerEnter)
            {
                Name.SetActive(true);
               
            }
            else
            {
                Name.SetActive(false);
            }
            info = m_Animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime > 1)
            {
                //Debug.Log("finish");
                OnFinishOpen();
                isFinish = true;
                Name.SetActive(false);
            }
        }
    }
    public virtual void SetDropItems(List<ItemData> items)
    {
        dropItems = items;
    }
    protected virtual void OnFinishOpen()
    {
        AudioManager.instance.PlaySFX(33, null);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&!isFinish)
        {
            isPlayerEnter = true;
            //player = collision.transform.GetComponent<Symbol>().GetCharacter() as IPlayer;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&!isFinish)
        {
            isPlayerEnter = false;
        }
    }
       

    public void TiggerAction()
    {
       if (isPlayerEnter&&!isFinish)
        {
            m_Animator.enabled = true;
        }
    }

    public bool IsInteractable()
    {
       return isPlayerEnter && !isFinish;
    }
}
