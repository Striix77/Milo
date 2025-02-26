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
    public LayerMask groundMask;

    public Animator animator;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 14f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    public TrailRenderer tr;

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        movX = Input.GetAxisRaw("Horizontal");
        movY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(movX) > 0)
        {
            rb.linearVelocity = new Vector2(movX * movementSpeed, rb.linearVelocity.y);

            float direction = Mathf.Sign(movX);
            transform.localScale = new Vector3(direction * 3, 3, 3);
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
        if (Mathf.Abs(movY) > 0 && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, movY * jumpSpeed);
            jumping = true;
            animator.SetBool("Jumping", jumping);

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
        // CheckForGround();
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

    // void CheckForGround()
    // {
    //     grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    // }

    void OnCollisionEnter2D(Collision2D collision2)
    {
        if (collision2.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            animator.SetBool("OnGround", grounded);
        }
    }

    void OnCollisionExit2D(Collision2D collision2)
    {
        if (collision2.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    void CheckForFalling()
    {
        falling = rb.linearVelocity.y < -0.1;
        animator.SetBool("Falling", falling);
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
