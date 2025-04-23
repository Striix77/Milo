using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public GameObject DeathSmokePrefab;

    void Update()
    {
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.1f);
        }
    }

    void DestroyEnemy()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        GameObject smoke = Instantiate(DeathSmokePrefab, transform.position, Quaternion.identity);
        Destroy(smoke, 0.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        Invoke(nameof(ResetColor), 0.1f);
    }

    void ResetColor()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
