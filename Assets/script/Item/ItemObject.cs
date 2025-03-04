using TMPro;
using UnityEngine;




public class ItemObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] protected LayerMask groundLayer;

    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;

    public TextMeshProUGUI text;
    public GameObject Name;
    float time;
    bool canflash;
    int x = 1;
    private ShopRoom shopRoom;
    public int needCoin;
    //[SerializeField] private Vector2 velocity;

    private void OnValidate()
    {
        SetupVisuals();

    }
    void OnEnable()
    {
     
    }
    void OnDisable()
    {
        shopRoom = null;
    }
    private void SetupVisuals()
    {
        if (itemData == null) return;
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item :" + itemData.itemName;
        text.text = itemData.itemName;
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        time = 25f;
        rb = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = itemData.itemIcon;
        //rb.velocity = new Vector2(0, 7);

    }

    // Update is called once per frame
    void Update()
    {
        if (IsGroundedDetected()&&time<24f)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        if (time >= 0)
        {
            time -= Time.deltaTime;
        }
        else if(shopRoom == null)
        {
            PoolMgr.Instance.Release(gameObject,5f);
            //Destroy(gameObject, 5f);
            canflash = true;

        }
        if (canflash && spriteRenderer != null)
        {

            //Debug.Log("flash");
            float alpha = spriteRenderer.color.a - 15f * Time.deltaTime * x;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            if (spriteRenderer.color.a <= 0)
            {
                x = -1;
            }
            if (spriteRenderer.color.a >= 1)
            {
                x = 1;
            }

        }

    }





    public void PickupItem()
    {
        if (shopRoom != null)  // 如果是商店的物品
        {
            // 将物品购买逻辑和商店关联
            if (!PlayerManager.instance.HaveEnoughCoin(needCoin))//shopRoom.TryBuyItem(itemData))
            {
                rb.velocity = new Vector2(0, 3);
                PlayerManager.instance.player.entityFX.CreatePopUpText("金币不足", Color.red);
                return;
            }
            UI.instance.itemText.gameObject.SetActive(true);
            UI.instance.itemText.SetupItemText(itemData);
            Inventory.instance.AddItem(itemData);
            PoolMgr.Instance.Release(gameObject);
            AudioManager.instance.PlaySFX(25, null);
        }
        else
        {
            if (!Inventory.instance.CanequipItem(itemData))
            {
                rb.velocity = new Vector2(0, 3);
                PlayerManager.instance.player.entityFX.CreatePopUpText("背包已满", Color.red);
                AudioManager.instance.PlaySFX(17, null);
                return;
            }
            UI.instance.itemText.gameObject.SetActive(true);
            UI.instance.itemText.SetupItemText(itemData);
            Inventory.instance.AddItem(itemData);
            PoolMgr.Instance.Release(gameObject);
            //AudioManager.instance.PlaySFX(25, null);
            AudioMgr.Instance.PlaySFX("3455_get1");
        }
        //Destroy(gameObject);
    }
    public void SetUpItem(ItemData itemData, Vector2 velocity, ShopRoom shopRoom= null)
    {
        this.itemData = itemData;
        rb.velocity = velocity;
        text.text = itemData.itemName;
        this.shopRoom = shopRoom;

        if (shopRoom != null)
        {
            needCoin= (int)(itemData.needMoney * (1 + shopRoom.Difficulty * 0.1) * 0.1f*Random.Range(0.9f, 1.1f));
            Name.gameObject.SetActive(false);
            Name = transform.Find("NameOfShop")?.gameObject;
            Name.GetComponent<ShopItemUI>().ShowToolTip(itemData as ItemDataEquipment, needCoin);
        }
        else
        {
            Name.gameObject.SetActive(false);
            Name = transform.Find("Name")?.gameObject;
        }
            SetupVisuals();
    }
    public virtual bool IsGroundedDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
}
