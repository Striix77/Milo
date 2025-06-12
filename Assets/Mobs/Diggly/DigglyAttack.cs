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
    public bool isAttacking = false;
    public float dmgModifier = 1f;



    void Start()
    {
        digglyAnimator = GetComponent<DigglyAnimator>();
    }

    void Update()
    {
        if (isAttacking)
        {
            Attack();
            // attackTimer += Time.deltaTime;
            // if (attackTimer >= digglyAnimator.disappearDelay)
            // {
            //     isAttacking = false;
            // }
        }

    }

    public void RestartAttack()
    {
        isAttacking = true;
        timer = 0f;
    }

    private void Attack()
    {
        timer += Time.deltaTime;
        if (timer >= rockSpawnRate)
        {
            GameObject rockPrefab = GetRandomRockPrefab();
            GameObject rock = Instantiate(rockPrefab, AttackPosition.position + new Vector3(GetRandomRange(-0.5f, 0.5f), 0, 0), Quaternion.identity);
            rock.GetComponent<EnemyProjectile>().damage = rock.GetComponent<EnemyProjectile>().damage * dmgModifier / 100;
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
        float angle = Random.Range(30, 150) * Mathf.Deg2Rad;
        Debug.DrawLine(AttackPosition.position, AttackPosition.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0), Color.red, 1f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }
}
