using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public float jumpSpeed;
    private Vector2 moveDirection;
    private float movX, movY;
    public Rigidbody2D rb;

    public float drag;
    public bool grounded;
    public bool jumping;
    public bool falling;
    public BoxCollider2D groundCheck;
    public CapsuleCollider2D playerCollider;
    public Vector2[] playerColliderCorners;
    public LayerMask groundMask;

    public Animator animator;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 14f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    public float dampingSpeed = 100f;

    private float coyoteTime = 5f;
    private float coyoteTimeCounter;

    public TrailRenderer tr;

    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerColliderCorners();
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (coyoteTimeCounter <= 0)
        {
            Debug.Log("Coyote time over");
        }
        if (isDashing)
        {
            return;
        }
        movX = Input.GetAxisRaw("Horizontal");
        movY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(movX) > 0 && !IsTouchingWall(movX))
        {
            rb.linearVelocity = new Vector2(movX * movementSpeed, rb.linearVelocity.y);

            float direction = Mathf.Sign(movX);
            transform.localScale = new Vector3(direction * 3, 3, 3);
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        if (movY > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, movY * jumpSpeed);
            jumping = true;
            animator.SetBool("Jumping", jumping);
            coyoteTimeCounter = 0;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        CheckForGround();
        CheckForFalling();
        if (grounded && movX == 0 && movY == 0)
        {
            rb.linearVelocity *= drag;
            falling = false;
        }
        if (falling)
        {
            jumping = false;
            animator.SetBool("Jumping", jumping);
        }
    }

    void CheckForGround()
    {
        grounded = Physics2D.Raycast((playerColliderCorners[0] + playerColliderCorners[1]) / 2, Vector2.down, 0.3f, groundMask);
    }

    // void OnCollisionEnter2D(Collision2D collision2)
    // {
    //     if (collision2.gameObject.CompareTag("Ground"))
    //     {
    //         grounded = true;
    //         animator.SetBool("OnGround", grounded);
    //     }
    // }

    // void OnCollisionExit2D(Collision2D collision2)
    // {
    //     if (collision2.gameObject.CompareTag("Ground"))
    //     {
    //         grounded = false;
    //     }
    // }

    private bool IsTouchingWall(float direction)
    {
        Debug.Log(direction);
        if (direction == 0)
        {
            return false;
        }

        if (direction == 1)
        {
            RaycastHit2D rightHit1 = Physics2D.Raycast(playerColliderCorners[1], Vector2.right * movX, 0.3f, groundMask);
            RaycastHit2D rightHit2 = Physics2D.Raycast(playerColliderCorners[2], Vector2.right * movX, 0.3f, groundMask);
            RaycastHit2D rightHit3 = Physics2D.Raycast((playerColliderCorners[1] + playerColliderCorners[2]) / 2, Vector2.right * movX, 0.3f, groundMask);

            return rightHit1.collider != null || rightHit2.collider != null || rightHit3.collider != null;
        }
        RaycastHit2D leftHit1 = Physics2D.Raycast(playerColliderCorners[0], Vector2.right * movX, 0.3f, groundMask);
        RaycastHit2D lefthit2 = Physics2D.Raycast(playerColliderCorners[3], Vector2.right * movX, 0.3f, groundMask);
        RaycastHit2D leftHit3 = Physics2D.Raycast((playerColliderCorners[0] + playerColliderCorners[3]) / 2, Vector2.right * movX, 0.3f, groundMask);

        return leftHit1.collider != null || lefthit2.collider != null || leftHit3.collider != null;
    }

    void CheckForFalling()
    {
        falling = rb.linearVelocity.y < -0.1;
        animator.SetBool("Falling", falling);
    }

    private void GetPlayerColliderCorners()
    {
        playerColliderCorners = new Vector2[4];
        Bounds bounds = playerCollider.bounds;
        playerColliderCorners[0] = new Vector2(bounds.min.x, bounds.min.y); // Bottom-left corner
        playerColliderCorners[1] = new Vector2(bounds.max.x, bounds.min.y); // Bottom-right corner
        playerColliderCorners[2] = new Vector2(bounds.max.x, bounds.max.y); // Top-right corner
        playerColliderCorners[3] = new Vector2(bounds.min.x, bounds.max.y); // Top-left corner

    }

    private IEnumerator Dash()
    {
        tr.emitting = true;
        canDash = false;
        isDashing = true;
        animator.SetBool("Dashing", true);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        animator.SetBool("Dashing", false);
        tr.emitting = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
