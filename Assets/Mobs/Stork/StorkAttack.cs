using UnityEngine;

public class StorkAttack : MonoBehaviour
{
    public GameObject StorkBombPrefab;
    public Transform StorkBombSpawnPoint;
    [Header("Attack Settings")]
    public float attackThreshold = 1.5f;
    public float attackCooldown = 2.0f;
    public float allowedAttacks = 1f;

    [Header("References")]
    private Transform playerTransform;
    private float lastAttackTime;

    private int storkNoBombAnimation = Animator.StringToHash("Stork");
    private Animator animator;
    private StorkMovement storkMovement;
    public float dmgModifier = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        storkMovement = GetComponent<StorkMovement>();
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure your player has the 'Player' tag.");
        }

        lastAttackTime = -attackCooldown; // Allow immediate attack on game start
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Check if stork is above player (comparing X positions)
        bool isAbovePlayer = Mathf.Abs(transform.position.x - playerTransform.position.x) < attackThreshold;

        // If above player and cooldown expired, attack
        if ((storkMovement.currentState.Equals(StorkState.MovingLeft) || storkMovement.currentState.Equals(StorkState.MovingRight)) && isAbovePlayer && Time.time > lastAttackTime + attackCooldown && allowedAttacks > 0)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    void PerformAttack()
    {
        allowedAttacks--;
        animator.CrossFade(storkNoBombAnimation, 0.1f, 0);
        GameObject storkBomb = Instantiate(StorkBombPrefab, StorkBombSpawnPoint.position, Quaternion.identity);
        storkBomb.GetComponent<EnemyProjectile>().damage += storkBomb.GetComponent<EnemyProjectile>().damage * dmgModifier / 100;
        Destroy(storkBomb, 5f);
    }
}