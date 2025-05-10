using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    public float health = 350f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Player Health: " + health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
    }
}
