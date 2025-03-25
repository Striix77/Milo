using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;

    void Update()
    {
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 1);
        }
    }

    void DestroyEnemy()
    {
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
