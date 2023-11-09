using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpStrength;
    private float xInput;
    private Rigidbody2D rb;
    private Animator anim;
    private bool facingRight = true;
    private int facingDir = 1;

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
        CheckInputs();
        FlipController();
        CheckCollisions();
        Animations();
    }

    private void CheckCollisions()
    {
        isGrounded = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, distanteToGround, groundLayer);
    }

    private void Movement()
    {
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void CheckInputs()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if(isGrounded)
            rb.velocity = new Vector2(xInput, jumpStrength);
    }

    private void Animations()
    {
        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
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
}
