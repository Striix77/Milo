using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementStats MovementStats;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private Collider2D feetCollider;

    private Rigidbody2D rb;

    // Movement
    private Vector2 velocity;
    private bool isFacingRight;

    // Collision
    private bool isGrounded;
    private bool isHeadBumped;
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;

    // Jump
    public float VerticalVelocity { get; private set; }
    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;


    // Jump Apex
    private float apexPoint;
    private float timePastApex;
    private bool isPastApex;

    // Jump Buffer
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    // Jump Coyote Time
    private float jumpCoyoteTimer;

    // Animations
    private Animator animator;

    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CountTimers();
        JumpChecks();
        AnimationControl();
    }

    private void FixedUpdate()
    {
        IsGrounded();
        BumpedHead();
        Jump();

        if (isGrounded)
        {
            Move(MovementStats.GroundAcceleration, MovementStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            Move(MovementStats.AirAcceleration, MovementStats.AirDeceleration, InputManager.Movement);
        }
    }

    // Movement
    private void Move(float acceleration, float deceleration, Vector2 movementInput)
    {
        if (movementInput.x != 0)
        {
            // check for direction
            TurnCheck(movementInput);

            Vector2 targetVelocity = new Vector2(movementInput.x, 0f) * MovementStats.MaxWalkSpeed;

            velocity = Vector2.Lerp(velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);
        }
        else
        {
            velocity = Vector2.Lerp(velocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 movementInput)
    {
        if (movementInput.x > 0 && !isFacingRight)
        {
            Turn(true);
        }
        else if (movementInput.x < 0 && isFacingRight)
        {
            Turn(false);
        }
    }


    private void Turn(bool shouldTurn)
    {
        if (shouldTurn)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    //Jump
    private void JumpChecks()
    {
        // Pressing the jump button

        if (InputManager.JumpWasPressed)
        {
            jumpBufferTimer = MovementStats.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        //Releasing the jump button

        if (InputManager.JumpWasReleased)
        {
            if (jumpBufferTimer > 0)
            {
                jumpReleasedDuringBuffer = true;
            }

            if (isJumping && VerticalVelocity > 0)
            {
                if (isPastApex)
                {
                    isPastApex = false;
                    isFastFalling = true;
                    fastFallTime = MovementStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        //Initiate jump with buffering and coyote time

        if (jumpBufferTimer > 0f && !isJumping && (isGrounded || jumpCoyoteTimer > 0f))
        {
            InitiateJump(1);

            // Bunny hop
            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        //Double jump

        else if (jumpBufferTimer > 0f && isJumping && numberOfJumpsUsed < MovementStats.NumberOfJumps)
        {
            isFastFalling = false;
            InitiateJump(1);
        }

        // Air jump after coyote

        else if (jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < MovementStats.NumberOfJumps - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }

        //Landed
        if ((isJumping || isFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApex = false;
            numberOfJumpsUsed = 0;
            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int jumpsUsed)
    {
        if (!isJumping)
        {
            isJumping = true;
        }

        jumpBufferTimer = 0;
        numberOfJumpsUsed += jumpsUsed;
        VerticalVelocity = MovementStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        // Apply gravity

        if (isJumping)
        {
            // Check for head bump

            if (isHeadBumped)
            {
                isFastFalling = true;
            }

            // Gravity on ascending

            if (VerticalVelocity >= 0f)
            {
                //Apex Controls

                apexPoint = Mathf.InverseLerp(MovementStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (apexPoint > MovementStats.ApexThreshold)
                {
                    if (!isPastApex)
                    {
                        timePastApex = 0f;
                        isPastApex = true;
                    }
                    else
                    {
                        timePastApex += Time.fixedDeltaTime;
                        if (timePastApex < MovementStats.ApexTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                // Gravity on descending but not past apex
                else
                {
                    VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
                    if (isPastApex)
                    {
                        isPastApex = false;
                    }
                }

            }

            //Gravity on descending

            else if (!isFastFalling)
            {
                VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f)
            {
                if (!isFalling)
                {
                    isFalling = true;
                }
            }



        }


        //Jump Cut

        if (isFastFalling)
        {
            if (fastFallTime >= MovementStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < MovementStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, fastFallTime / MovementStats.TimeForUpwardsCancel);
            }
            fastFallTime += Time.fixedDeltaTime;
        }

        //Normal gravity while falling
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
        }

        //Clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementStats.MaxFallSpeed, 50f);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, VerticalVelocity);
    }

    // Collision

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(bodyCollider.bounds.center.x, bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(bodyCollider.bounds.size.x * MovementStats.HeadWidth, MovementStats.HeadDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MovementStats.HeadDetectionRayLength, MovementStats.GroundLayer);
        if (headHit.collider != null)
        {
            isHeadBumped = true;
        }
        else
        {
            isHeadBumped = false;
        }

        //Visualize Head Detection Ray
        if (MovementStats.DebugHeadBump)
        {
            Color rayColor = isHeadBumped ? Color.green : Color.red;
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * MovementStats.HeadWidth, boxCastOrigin.y), Vector2.up * MovementStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2 * MovementStats.HeadWidth, boxCastOrigin.y), Vector2.up * MovementStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * MovementStats.HeadWidth, boxCastOrigin.y + MovementStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x, MovementStats.GroundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MovementStats.GroundDetectionRayLength, MovementStats.GroundLayer);
        if (groundHit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //Visualize Ground Detection Ray
        if (MovementStats.DebugGroundDetection)
        {
            Color rayColor = isGrounded ? Color.green : Color.red;
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MovementStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MovementStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MovementStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }

    }

    // Timers

    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;

        if (!isGrounded)
        {
            jumpCoyoteTimer -= Time.deltaTime;
        }
        else
        {
            jumpCoyoteTimer = MovementStats.JumpCoyoteTime;
        }
    }

    // Animations
    private void AnimationControl()
    {
        if (isGrounded)
        {
            animator.SetBool("OnGround", true);
        }
        else
        {
            animator.SetBool("OnGround", false);
        }
        if (InputManager.Movement.x != 0 && isGrounded)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
        if (isJumping)
        {
            animator.SetBool("Jumping", true);
        }
        else
        {
            animator.SetBool("Jumping", false);
        }
        if (isFalling || isFastFalling || isPastApex)
        {
            Debug.Log("Falling");
            animator.SetBool("Falling", true);
        }
        else
        {
            animator.SetBool("Falling", false);
        }
    }
}
