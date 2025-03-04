using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;

    protected int facingDirection = 1;
    protected bool facingRight = true;

    [Header("Ground Check")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float checkgroundRadius;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected float checkwallRadius;
    protected bool isGrounded;
    protected bool isTouchingWall;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        if(wallCheck == null)
        {
            wallCheck = transform;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CollisionChecks();
    }
    protected virtual void CollisionChecks()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, checkgroundRadius, groundLayer);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right, checkwallRadius*facingDirection, wallLayer);
       
    }
    protected virtual void Flip()
    {

        facingDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - checkgroundRadius));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + checkwallRadius*facingDirection, wallCheck.position.y));
       
    }
}
