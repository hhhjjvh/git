using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton : Entity
{
    private bool isAttacking;
    [Header("Move info")]
    [SerializeField] private float moveSpeed;

    [Header("player detection")]
    [SerializeField] private float playerCheckDistance;
    [SerializeField] private LayerMask playerLayer;

    private RaycastHit2D isplayerDetected;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        if (isplayerDetected.distance>1)
        {
            rb.velocity = new Vector2(moveSpeed * facingDirection*1.5f, rb.velocity.y);
            //isAttacking = false;
        }
        else
        {
            //isAttacking = true;
        }


        if (!isGrounded || isTouchingWall)
        {
            Flip();
        }
        Movement();
    }

    private void Movement()
    {
        rb.velocity = new Vector2(moveSpeed * facingDirection, rb.velocity.y);
    }

    protected override void CollisionChecks()
    {
        base.CollisionChecks();
        isplayerDetected = Physics2D.Raycast(transform.position, Vector2.right , playerCheckDistance * facingDirection, playerLayer);
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x+playerCheckDistance*facingDirection,transform.position.y));
    }
}
