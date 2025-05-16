using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Scriptable Objects/Player Movement Stats")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Movement")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 10f;
    [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float GroundDeceleration = 20f;
    [Range(0.25f, 50f)] public float AirAcceleration = 5f;
    [Range(0.25f, 50f)] public float AirDeceleration = 5f;

    [Header("Grounded/Collision Checks")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    public bool DebugGroundDetection = true;
    public bool DebugHeadBump = true;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;

    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float JumpHeightCompensation = 1.05f;
    public float TimeTillJumpApex = 0.4f;
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    [Range(1, 5)] public int NumberOfJumps = 2;
    [Range(1f, 5f)] public float PositiveVelocityRotationSpeed = 5f;
    [Range(1f, 5f)] public float NegativeVelocityRotationSpeed = 5f;
    public bool EnableJumpRotation = true;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.03f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.9f;
    [Range(0.01f, 1f)] public float ApexTime = 0.1f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.15f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

    [Header("Jump Visualization")]
    public bool ShowWalkJumpArc = false;
    public bool StopOnCollision = true;
    public bool DrawRight = true;
    [Range(5, 100)] public int ArcResolution = 20;
    [Range(0, 500)] public int VisualizationSteps = 90;

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    public float AdjustedJumpHeight { get; private set; }

    [Header("Dashing")]
    [Range(0f, 1f)] public float DashTime = 0.11f;
    [Range(1f, 200f)] public float DashSpeed = 40f;
    [Range(0f, 5f)] public float DashCooldown = 0.5f;
    [Range(0, 5)] public int NumberOfDashes = 2;
    [Range(0f, 0.5f)] public float DashDiagonalBias = 0.4f;
    [Range(0.01f, 5f)] public float DashGravityOnReleaseMultiplier = 1f;
    [Range(0.02f, 0.3f)] public float DashTimeForUpwardsCancel = 0.027f;
    [Range(0f, 1f)] public float DashMomentumRetention = 0.5f;
    public static bool createGhost = false;

    public readonly Vector2[] DashDirections = new Vector2[]
    {
        new Vector2(0, 0), // No Dash
        new Vector2(1, 0), // Right
        new Vector2(1, 1).normalized, // Right Up
        new Vector2(0, 1), // Up
        new Vector2(-1, 1).normalized, // Left Up
        new Vector2(-1, 0), // Left
        new Vector2(-1, -1).normalized, // Left Down
        new Vector2(0, -1), // Down
        new Vector2(1, -1).normalized, // Right Down
    };



    private void OnValidate()
    {
        CalculateGravityAndJump();
    }

    private void OnEnable()
    {
        CalculateGravityAndJump();
    }

    private void CalculateGravityAndJump()
    {
        AdjustedJumpHeight = JumpHeight * JumpHeightCompensation;
        Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }
}
