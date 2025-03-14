using UnityEngine;

public class Tail : MonoBehaviour
{
    public int Length;
    public LineRenderer lineRenderer;
    public Vector3[] SegmentPos;
    public Vector3[] SegmentV;

    public Transform TargetDir;
    public float TargetDist;
    public float SmoothSpeed;

    public Transform PlayerTransform;
    private Vector2 origTextureScale;

    // New mouse following parameters
    public float MouseFollowSpeed = 5.0f;
    public Camera mainCamera;
    private Vector3 lastSegmentVelocity;

    // Jump offset
    [SerializeField][Range(0.0f, 0.3f)] private float jumpOffset = 0.05f;
    [SerializeField][Range(0.0f, 0.3f)] private float fallOffset = 0.05f;
    private Vector3 originalTargetDirLocalPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origTextureScale = lineRenderer.textureScale;
        lineRenderer.positionCount = Length;
        SegmentPos = new Vector3[Length];
        SegmentV = new Vector3[Length];

        // Initialize all positions
        for (int i = 0; i < Length; i++)
        {
            SegmentPos[i] = TargetDir.position;
        }

        // If camera not assigned, try to find the main camera
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Store original position of target dir
        if (TargetDir != null)
            originalTargetDirLocalPos = TargetDir.localPosition;


        lastSegmentVelocity = Vector3.zero;
    }

    void Update()
    {
        SetTextureWithDirection();
        OffsetTailForJump();
        TailMouseFollow();
    }

    private void SetTextureWithDirection()
    {
        Debug.Log(origTextureScale);
        if (PlayerTransform == null)
            return;
        if (PlayerTransform.rotation.y > 0)
        {
            lineRenderer.textureScale = new Vector2(origTextureScale.x, origTextureScale.y);
        }
        else
        {
            lineRenderer.textureScale = new Vector2(origTextureScale.x, -origTextureScale.y);
        }
    }

    private void OffsetTailForJump()
    {
        if (TargetDir == null)
            return;

        // If player is jumping, move the tail up
        if (PlayerAnimations.GetState() == PlayerAnimations.Jump)
        {
            TargetDir.localPosition = originalTargetDirLocalPos + Vector3.up * jumpOffset;
        }
        else if (PlayerAnimations.GetState() == PlayerAnimations.Fall)
        {
            TargetDir.localPosition = originalTargetDirLocalPos + Vector3.up * fallOffset;
        }
        else
        {
            TargetDir.localPosition = originalTargetDirLocalPos;
        }
    }

    private void TailMouseFollow()
    {
        // First segment follows the target as before
        SegmentPos[0] = TargetDir.position;

        // Move last segment towards mouse
        if (mainCamera != null)
        {
            // Convert mouse position to world space
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Vector3.Distance(mainCamera.transform.position, transform.position); // Distance from camera
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mousePos);

            // Smooth movement of last segment towards mouse
            SegmentPos[Length - 1] = Vector3.SmoothDamp(
                SegmentPos[Length - 1],
                mouseWorldPos,
                ref lastSegmentVelocity,
                1.0f / MouseFollowSpeed);
        }

        // Calculate total intended length
        float intendedLength = TargetDist * (Length - 1);

        // Limit the last segment's position based on distance from first segment
        Vector3 directionToLast = (SegmentPos[Length - 1] - SegmentPos[0]).normalized;
        float currentDistance = Vector3.Distance(SegmentPos[0], SegmentPos[Length - 1]);

        if (currentDistance > intendedLength)
        {
            SegmentPos[Length - 1] = SegmentPos[0] + directionToLast * intendedLength;
        }

        // Update middle segments with bidirectional constraints
        // First pass: front-to-back
        for (int i = 1; i < Length - 1; i++)
        {
            Vector3 targetPosition = SegmentPos[i - 1] - (SegmentPos[i - 1] - SegmentPos[i]).normalized * TargetDist;
            SegmentPos[i] = Vector3.SmoothDamp(SegmentPos[i], targetPosition, ref SegmentV[i], SmoothSpeed);
            SegmentPos[i] = SegmentPos[i - 1] + (SegmentPos[i] - SegmentPos[i - 1]).normalized * TargetDist;
        }

        // Second pass: back-to-front to ensure the tail follows the last segment properly
        for (int i = Length - 2; i >= 1; i--)
        {
            SegmentPos[i] = SegmentPos[i + 1] + (SegmentPos[i] - SegmentPos[i + 1]).normalized * TargetDist;
        }

        lineRenderer.SetPositions(SegmentPos);
    }

    private void MoveTailWithAnimations()
    {

    }
}