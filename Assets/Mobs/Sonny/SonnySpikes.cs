using UnityEngine;

public class SonnySpikes : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 200f;
    public float followDuration = 5f;
    public float minDistanceToPlayer = 0.5f;
    public float followStartTime = 0f;
    private float followStartTimer = 0f;

    [Header("Avoidance Settings")]
    public float avoidanceRadius = 0.75f; // How close spikes can get to each other
    public float avoidanceStrength = 1.5f;


    [Header("References")]
    private Transform playerTransform;
    private float timer;
    private bool isFollowing = false;



    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure it has the 'Player' tag.");
            isFollowing = false;
        }

        timer = followDuration;
    }

    private void Update()
    {

        if (playerTransform == null)
            return;

        if (isFollowing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isFollowing = false;
                Destroy(gameObject);
            }

            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

            Vector2 avoidanceDirection = CalculateAvoidanceDirection();

            Vector2 moveDirection = directionToPlayer + avoidanceDirection;
            moveDirection.Normalize();

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > minDistanceToPlayer)
            {

                transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;


                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            followStartTimer += Time.deltaTime;
            if (followStartTimer >= followStartTime)
            {
                isFollowing = true;
            }
            transform.position += Vector3.up * 1 * Time.deltaTime;
        }
    }

    private Vector2 CalculateAvoidanceDirection()
    {
        Vector2 avoidanceDirection = Vector2.zero;

        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);

        foreach (Collider2D nearbyObject in nearbyObjects)
        {
            if (nearbyObject.gameObject == gameObject || !nearbyObject.CompareTag("Spike"))
                continue;

            Vector2 directionAway = (Vector2)transform.position - (Vector2)nearbyObject.transform.position;
            float distance = directionAway.magnitude;

            if (distance > 0)
            {
                float strength = (avoidanceRadius - distance) / avoidanceRadius;
                avoidanceDirection += directionAway.normalized * strength * avoidanceStrength;
            }
        }

        return avoidanceDirection;
    }

    
}
