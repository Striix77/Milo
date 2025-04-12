using UnityEngine;

public class DigglyAttack : MonoBehaviour
{

    private DigglyAnimator digglyAnimator;
    public Transform AttackPosition;
    public GameObject RockPrefab1;
    public GameObject RockPrefab2;
    public GameObject RockPrefab3;

    public float DestroyDelay = 1f;
    public float rockForce = 5f;
    public float rockSpawnRate = 1f; // Time in seconds between rock spawns
    private float timer = 0f;
    private float attackTimer = 0f;
    public bool isAttacking = false;



    void Start()
    {
        digglyAnimator = GetComponent<DigglyAnimator>();
    }

    void Update()
    {
        Debug.Log("ISDAALKEJWNOIUKGAS:JDUOJDEGABHUA: " + isAttacking);
        if (isAttacking)
        {
            Attack();
            attackTimer += Time.deltaTime;
            if (attackTimer >= digglyAnimator.disappearDelay)
            {
                isAttacking = false;
            }
        }

    }

    private void Attack()
    {
        timer += Time.deltaTime;
        if (timer >= rockSpawnRate)
        {
            GameObject rockPrefab = GetRandomRockPrefab();
            GameObject rock = Instantiate(rockPrefab, AttackPosition.position + new Vector3(GetRandomRange(-1, 1), 0, 0), Quaternion.identity);
            rock.GetComponent<Rigidbody2D>().AddForce(GetRandomDirection() * rockForce, ForceMode2D.Impulse);
            Destroy(rock, DestroyDelay);
            timer = 0f;
        }

    }

    private float GetRandomRange(float min, float max)
    {
        return Random.Range(min, max);
    }

    private GameObject GetRandomRockPrefab()
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                return RockPrefab1;
            case 1:
                return RockPrefab2;
            case 2:
                return RockPrefab3;
            default:
                return RockPrefab1;
        }
    }

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0, 140);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }
}
