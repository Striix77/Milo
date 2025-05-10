using UnityEngine;

public class SonnyAnimator : MonoBehaviour
{
    private int sonnyDashStartHash = Animator.StringToHash("SonnyDashStart");
    private int sonnyDashEndHash = Animator.StringToHash("SonnyDashEnd");
    private int sonnyDashHash = Animator.StringToHash("SonnyDash");
    private int sonnySpikingUpHash = Animator.StringToHash("SonnySpikingUp");

    [Header("Dash Settings")]
    public float dashTime = 0.5f;
    public float dashSpeed = 5f;
    public float playerDetectionRange = 10f;

    [Header("Spike Settings")]
    public float spikeTime = 0.5f;
    public float spikeTimer = 0f;
    public GameObject SpikePrefab1;
    public GameObject SpikePrefab2;
    public Transform SpikeSpawnPoint;

    private float dashTimeCounter = 0f;
    public bool isDashing = false;
    public bool isSpiking = false;
    public bool canThrowSpikes = true;
    private Vector2 dashDirection;
    private Transform playerTransform;
    private Animator animator;
    private Collider2D sonnyCollider2D;



    void Start()
    {
        animator = GetComponent<Animator>();
        sonnyCollider2D = GetComponent<Collider2D>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure it has the 'Player' tag.");
        }
    }

    void Update()
    {
        CheckDashAnimation();
        CheckSpikingUpAnimation();

        if (!isDashing && ShouldStartDash())
        {
            StartDash();
        }

        if (isDashing)
        {
            PerformDash();
            dashTimeCounter += Time.deltaTime;
            if (dashTimeCounter >= dashTime || Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                EndDash();
            }
        }

        if (isSpiking && canThrowSpikes)
        {
            spikeTimer += Time.deltaTime;
            if (spikeTimer >= spikeTime)
            {
                isSpiking = false;
                spikeTimer = 0f;
                for (int i = 0; i <= 2; i++)
                {
                    int randomIndex = Random.Range(0, 2);
                    float randomSizeMultiplier = Random.Range(1f, 2f);
                    float randomPositionOffset = Random.Range(-0.5f, 0.5f);
                    if (randomIndex == 0)
                    {
                        GameObject spike = Instantiate(SpikePrefab1, SpikeSpawnPoint.position + new Vector3(randomPositionOffset, 0f, 0f), Quaternion.identity);
                        spike.transform.localScale = new Vector3(randomSizeMultiplier, randomSizeMultiplier, 1f);
                    }
                    else
                    {
                        GameObject spike = Instantiate(SpikePrefab2, SpikeSpawnPoint.position + new Vector3(randomPositionOffset, 0f, 0f), Quaternion.identity);
                        spike.transform.localScale = new Vector3(randomSizeMultiplier, randomSizeMultiplier, 1f);
                    }
                }
                canThrowSpikes = false;
            }
        }
    }

    private bool ShouldStartDash()
    {
        if (playerTransform == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= playerDetectionRange;
    }

    private void StartDash()
    {
        // sonnyCollider2D.enabled = false;



        dashTimeCounter = 0f;

        if (dashDirection.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void PerformDash()
    {
        if (playerTransform != null)
        {
            dashDirection = (playerTransform.position - transform.position).normalized;
        }
        else
        {
            dashDirection = transform.right;
        }
        Vector2 avoidance = CalculateAvoidanceDirection();
        dashDirection = (dashDirection * 0.7f) + (avoidance * 0.3f);
        dashDirection.Normalize();
        Vector2 dashMovement = dashDirection * dashSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + new Vector3(dashMovement.x, dashMovement.y, 0);
        transform.position = newPosition;
    }
    private Vector2 CalculateAvoidanceDirection()
    {
        Vector2 avoidanceDirection = Vector2.zero;

        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        foreach (Collider2D nearbyObject in nearbyObjects)
        {
            if (nearbyObject.gameObject == gameObject || !nearbyObject.gameObject.name.Contains("Sonny"))
                continue;

            Vector2 directionAway = (Vector2)transform.position - (Vector2)nearbyObject.transform.position;
            float distance = directionAway.magnitude;

            if (distance > 0)
            {
                float strength = (0.5f - distance) / 0.5f;
                avoidanceDirection += directionAway.normalized * strength;
            }
        }

        return avoidanceDirection;
    }

    private void EndDash()
    {
        dashTimeCounter = 0f;
        animator.CrossFade(sonnyDashEndHash, 0, 0);
        isDashing = false;
        canThrowSpikes = true;
        // sonnyCollider2D.enabled = true;
    }

    private void CheckDashAnimation()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            isDashing = stateInfo.shortNameHash == sonnyDashHash;
            sonnyCollider2D.enabled = !isDashing;

        }
    }

    public void CheckSpikingUpAnimation()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            isSpiking = stateInfo.shortNameHash == sonnySpikingUpHash;
        }
    }

}
