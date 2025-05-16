using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementStats MovementStats;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private GameObject visualContainer;

    private Rigidbody2D rb;

    // Movement
    public float HorizontalVelocity { get; private set; }
    private bool isFacingRight;

    // Collision
    public static bool isGrounded;
    private bool isHeadBumped;
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;

    // Jump
    public float VerticalVelocity { get; private set; }
    public static bool isJumping;
    public static bool isFastFalling;
    public static bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;
    public Image jumpImage;
    public Sprite jumpSprite;
    public Sprite jumpCooldownSprite;


    // Jump Apex
    private float apexPoint;
    private float timePastApex;
    public static bool isPastApex;

    // Jump Buffer
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    // Jump Coyote Time
    private float jumpCoyoteTimer;

    // Dashing
    public static bool isDashing;
    private bool isAirDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private int numberOfDashesUsed;
    private Vector2 dashDirection;
    public static bool isDashFastFalling;
    private float dashFastFallTime;
    private float dashFastFallReleaseSpeed;
    public Image dashImage;
    public TextMeshProUGUI dashText;
    public Sprite dashSprite;
    public Sprite dashCooldownSprite;


    // Animations
    private Animator animator;


    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        animator = visualContainer.GetComponentInChildren<Animator>();
        dashText.text = "";
    }

    private void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MiloStart"))
        {
            CountTimers();
            JumpChecks();
            LandCheck();

            DashCheck();
        }
    }

    private void FixedUpdate()
    {
        IsGrounded();
        BumpedHead();
        Jump();
        Fall();
        Dash();
        ApplyVelocityRotation();
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MiloStart"))
        {
            if (isGrounded)
            {
                Move(MovementStats.GroundAcceleration, MovementStats.GroundDeceleration, InputManager.Movement);
            }
            else
            {
                Move(MovementStats.AirAcceleration, MovementStats.AirDeceleration, InputManager.Movement);
            }
        }
        ApplyVelocity();
    }

    // Movement
    private void Move(float acceleration, float deceleration, Vector2 movementInput)
    {
        if (!isDashing)
        {
            Debug.Log("Moving: " + movementInput.y + " " + movementInput.x);
            if (movementInput.x != 0)
            {
                // check for direction
                TurnCheck(movementInput);

                float targetVelocity = movementInput.x * MovementStats.MaxWalkSpeed;

                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            }
            else
            {
                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
            }
        }
    }

    private void ApplyVelocity()
    {
        //Clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementStats.MaxFallSpeed, 50f);
        rb.linearVelocity = new Vector2(HorizontalVelocity, VerticalVelocity);
    }

    private void LandCheck()
    {
        //Landed
        if ((isJumping || isFalling || isDashFastFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApex = false;
            numberOfJumpsUsed = 0;
            VerticalVelocity = Physics2D.gravity.y;
            ResetDashes();

            //Reset rotation after landing
            visualContainer.transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);

            if (isDashFastFalling && isGrounded)
            {
                ResetDashValues();
                return;
            }
            ResetDashValues();
        }
    }

    private void Fall()
    {
        //Normal gravity while falling
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
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

        else if (jumpBufferTimer > 0f && (isJumping || isAirDashing || isDashFastFalling) && numberOfJumpsUsed < MovementStats.NumberOfJumps)
        {
            isFastFalling = false;
            isFalling = false;
            InitiateJump(1);

            if (isDashFastFalling)
            {
                isDashFastFalling = false;
            }
        }

        // Air jump after coyote

        else if (jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < MovementStats.NumberOfJumps - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }

        if (numberOfJumpsUsed >= MovementStats.NumberOfJumps)
        {
            jumpImage.color = new Color(0.781132f, 0.781132f, 0.781132f, 1f);
            jumpImage.sprite = jumpCooldownSprite;
        }
        else
        {
            jumpImage.color = new Color(1, 1, 1, 1);
            jumpImage.sprite = jumpSprite;
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
                else if (!isFastFalling)
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



        //Clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementStats.MaxFallSpeed, 50f);
    }

    private void ApplyVelocityRotation()
    {
        if (!MovementStats.EnableJumpRotation || isGrounded)
            return;

        if (isJumping || isFalling || isDashFastFalling)
        {
            // Calculate rotation angle based on velocity
            float targetRotation = 0f;

            // Calculate angle from velocity
            if (Mathf.Abs(VerticalVelocity) > 0.1f)
            {
                // Calculate angle in degrees from velocity vector
                targetRotation = Mathf.Atan2(VerticalVelocity, HorizontalVelocity) * Mathf.Rad2Deg;

                // Adjust based on facing direction
                if (!isFacingRight)
                    targetRotation = 180 - targetRotation;
                // Create rotation based on velocity direction
                Quaternion newRotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, targetRotation);

                // Smoothly interpolate to the target rotation unless Milo is dashing
                if (isDashing)
                { visualContainer.transform.rotation = newRotation; }
                else
                {
                    if (VerticalVelocity > 0)
                        visualContainer.transform.rotation = Quaternion.Slerp(visualContainer.transform.rotation, newRotation, MovementStats.PositiveVelocityRotationSpeed * Time.fixedDeltaTime);
                    else
                    {
                        visualContainer.transform.rotation = Quaternion.Slerp(visualContainer.transform.rotation, newRotation, MovementStats.NegativeVelocityRotationSpeed * Time.fixedDeltaTime);
                    }
                }
            }

        }
    }

    private void ResetJumpValues()
    {
        isJumping = false;
        isFalling = false;
        isFastFalling = false;
        fastFallTime = 0f;
        // fastFallReleaseSpeed = VerticalVelocity;
        // numberOfJumpsUsed = 0;
        isPastApex = false;
    }

    //Dashing

    private void DashCheck()
    {
        if (InputManager.DashWasPressed && dashCooldownTimer < 0)
        {
            //ground dash

            if (isGrounded && !isDashing)
            {
                InitiateDash();
            }

            //air dash

            else if (!isGrounded && !isDashing && numberOfDashesUsed < MovementStats.NumberOfDashes)
            {
                isAirDashing = true;
                InitiateDash();
            }
        }
    }

    private void InitiateDash()
    {
        dashDirection = InputManager.Movement;


        Vector2 closestDirection = Vector2.zero;
        float closestDistance = Vector2.Distance(dashDirection, MovementStats.DashDirections[0]);

        foreach (Vector2 direction in MovementStats.DashDirections)
        {
            //skip if we hit it dead on
            if (direction == dashDirection)
            {
                closestDirection = direction;
                break;
            }

            float distance = Vector2.Distance(dashDirection, direction);

            // check if dash is diagonal and apply bias

            bool isDisagonal = Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1;
            if (isDisagonal)
            {
                distance -= MovementStats.DashDiagonalBias;
            }

            else if (distance < closestDistance)
            {
                closestDistance = distance;
                closestDirection = direction;
            }
        }

        // handle no input
        if (closestDirection == Vector2.zero)
        {
            if (isFacingRight)
            {
                closestDirection = Vector2.right;
            }
            else
            {
                closestDirection = Vector2.left;
            }
        }

        dashDirection = closestDirection;
        numberOfDashesUsed++;
        isDashing = true;
        dashTimer = 0f;
        dashCooldownTimer = MovementStats.DashCooldown;

        ResetJumpValues();
    }

    private void Dash()
    {
        if (isDashing)
        {
            animator.SetBool("Dashing", true);
            PlayerMovementStats.createGhost = true;
            //stop dash after timer
            dashTimer += Time.fixedDeltaTime;
            if (dashTimer >= MovementStats.DashTime)
            {
                animator.SetBool("Dashing", false);
                if (isGrounded)
                {
                    ResetDashes();
                }

                isAirDashing = false;
                isDashing = false;
                PlayerMovementStats.createGhost = false;

                if (!isJumping)
                {
                    dashFastFallTime = 0f;
                    dashFastFallReleaseSpeed = VerticalVelocity;


                    if (!isGrounded)
                    {
                        isDashFastFalling = true;
                    }
                }

                return;
            }

            HorizontalVelocity = MovementStats.DashSpeed * dashDirection.x;

            if (dashDirection.y != 0f || isAirDashing)
            {
                VerticalVelocity = MovementStats.DashSpeed * dashDirection.y;
            }

        }

        //Handle dash cut time
        else if (isDashFastFalling)
        {
            if (VerticalVelocity > 0f)
            {
                if (dashFastFallTime >= MovementStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
                else if (dashFastFallTime < MovementStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(dashFastFallReleaseSpeed, 0f, dashFastFallTime / MovementStats.DashTimeForUpwardsCancel);
                }
                dashFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }


        }
    }

    private void ResetDashValues()
    {
        isDashFastFalling = false;
        // dashOnGroundTimer = -0.01f;
    }

    private void ResetDashes()
    {
        numberOfDashesUsed = 0;
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

        dashCooldownTimer -= Time.deltaTime;
        if (dashCooldownTimer < 0)
        {
            dashText.text = "";
            dashImage.color = new Color(1, 1, 1, 1);
            dashImage.sprite = dashSprite;
        }
        else
        {
            dashText.text = Mathf.Ceil(dashCooldownTimer).ToString();
            dashImage.color = new Color(0.781132f, 0.781132f, 0.781132f, 1);
            dashImage.sprite = dashCooldownSprite;
        }

        if (!isGrounded)
        {
            jumpCoyoteTimer -= Time.deltaTime;
        }
        else
        {
            jumpCoyoteTimer = MovementStats.JumpCoyoteTime;
        }
    }


}
