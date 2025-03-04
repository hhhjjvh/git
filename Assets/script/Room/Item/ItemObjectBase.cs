using UnityEngine;

public class ItemObjectBase : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    float time;
    bool canflash;
    int x = 1;



    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        time = 25f;
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (IsGroundedDetected() && time < 24f)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        if (time >= 0)
        {
            time -= Time.deltaTime;
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

    public void SetUpItem(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
    public virtual bool IsGroundedDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
    }
}
