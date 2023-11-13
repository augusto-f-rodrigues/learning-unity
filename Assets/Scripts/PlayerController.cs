    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpStrength;
    private float xInput;
    private Rigidbody2D rb;
    private Animator anim;
    private bool facingRight = true;
    private int facingDir = 1;

    [Header("Attack info")]
    [SerializeField] private bool isAttacking;
    [SerializeField] private int comboAttack;
    [SerializeField] private float comboCooldown;
    private float comboCooldownTimer;

    [Header("Dash info")]
    [SerializeField] private float dashForce;
    private float dashTimer;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;

    [Header("Colliders info")]
    [SerializeField] private float distanteToGround;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Movement();
        Timers();
        CheckInputs();
        FlipController();
        Animations();
        CheckCollisions();
    }

    private void CheckCollisions()
    {
        isGrounded = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, distanteToGround, groundLayer);
    }

    private void Movement()
    {
        if (isAttacking)
            rb.velocity = Vector2.zero;
        else if (dashTimer > 0 && !isAttacking)
            rb.velocity = new Vector2(facingDir * dashForce, 0);
        else
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void CheckInputs()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            DashAbility();

        if (Input.GetKeyDown(KeyCode.Mouse0) && dashTimer < 0)        
            AttackAbility();        
    }

    private void AttackAbility()
    {
        if (comboCooldownTimer < 0)
            comboAttack = 0;

        isAttacking = true;
        comboCooldownTimer = comboCooldown;

        if (comboAttack > 2)
            comboAttack = 0;
    }

    private void DashAbility()
    {
        if(dashCooldownTimer < 0)
        {
            dashCooldownTimer = dashCooldown;
            dashTimer = dashDuration;
        }
    }

    private void Timers()
    {
        dashTimer -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        comboCooldownTimer -= Time.deltaTime;
    }

    private void Jump()
    {
        if(isGrounded && !isAttacking)
            rb.velocity = new Vector2(xInput, jumpStrength);
    }

    private void Animations()
    {
        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);

        anim.SetFloat("yVelocity", rb.velocity.y);

        anim.SetBool("isGrounded", isGrounded);

        anim.SetBool("isDashing", dashTimer > 0);

        anim.SetBool("isAttacking", isAttacking && isGrounded);
        anim.SetInteger("comboAttack", comboAttack);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180 * facingDir, 0);
        facingDir = facingDir * -1;
    }

    private void FlipController()
    {
        if (xInput > 0 && !facingRight)
            Flip();
        else if (xInput < 0 && facingRight)
            Flip();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - distanteToGround));
    }

    public void AttackOver()
    {
        comboAttack++;
        isAttacking = false;
    }
}
