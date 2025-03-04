using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Movement info")]
    [SerializeField] private float movespeed;
    [SerializeField] private float jumpforce;

    [Header("Dash info")]
    [SerializeField] private float dashspeed;
     private float dashtime;
    [SerializeField] private float dashduration;
    [SerializeField] private float dashcooldown;
     private float dashcooldownTimer;

    [Header("Attack info")]
    private float comboTimer;
    private float attackduration=.3f;
    private bool isAttacking;
    private int comboCounter;


    private bool ismoving;
    private bool isjumping;
    private bool isdashing;
    private float moveInput;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
       
    }

    // Update is called once per frame
    protected override void Update()
    {
        Movement();
        Checkinput();
        CollisionChecks();
        Timerupdate();
        AnimatorControllers();
        FlipControllers();
    }

    private void Dash()
    {
        
        if ( dashcooldownTimer <= 0&&!isAttacking)
        {
            
            dashtime = dashduration;
            dashcooldownTimer = dashcooldown;
            isdashing = true;
        }
       
    }
    private void Timerupdate()
    {
        dashcooldownTimer -= Time.deltaTime;
        dashtime -= Time.deltaTime;
        comboTimer -= Time.deltaTime;
        if (dashtime <= 0)
        {
            isdashing = false;

        }
       
    }

   
    private void Checkinput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.K))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.L))
        { 
            Dash();
        }
        if (Input.GetKey(KeyCode.J)&&!isdashing)
        {
            startattack();
        }
      

    }
    private void startattack()
    {
        if (!isGrounded) return;
        isAttacking = true;
       
        if (comboTimer <= 0)
        {
            comboCounter = 0;
        }
        comboTimer = attackduration;
    }


    public void AttackOver()
    {
        isAttacking = false;
        comboCounter++;
        if (comboCounter > 2)
        {
            comboCounter = 0;
        }
    }
    private void Movement()
    {
        if(isAttacking)
        {
            rb.velocity = Vector2.zero;
        }
        else if (dashtime > 0)
        {
            rb.velocity = new Vector2(facingDirection * dashspeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(moveInput * movespeed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        if (isGrounded&&!isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
           
        }
    }

    private void  AnimatorControllers()
    {
        ismoving = rb.velocity.x != 0;
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("ismove", ismoving);
        anim.SetBool("isGround", isGrounded);
        anim.SetBool("isDashing", dashtime > 0);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("comboCounter", comboCounter);

    }
   
    private void FlipControllers()
    {
        if (facingRight && moveInput < 0 || !facingRight && moveInput > 0)
        {
            if (!isdashing)
                Flip();
        }
    }
    
}
