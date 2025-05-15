using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour
{
    public float maxHealth = 350f;
    public Slider healthSlider;
    public float health = 350f;
    private bool takingDamage = false;

    [Range(0f, 5f)] public float healthLerpSpeed = 2f;
    private float targetHealth;

    private PlayerAbilities playerAbilities;

    void Start()
    {
        health = maxHealth;
        targetHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        playerAbilities = GetComponent<PlayerAbilities>();
    }

    void Update()
    {
        if (healthSlider.value != targetHealth)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, Time.deltaTime * healthLerpSpeed);

            if (Mathf.Abs(healthSlider.value - targetHealth) < 0.1f)
            {
                healthSlider.value = targetHealth;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!playerAbilities.isUlting)
        {
            if (health >= 0)
            {
                health -= damage;
                targetHealth = health;
                Debug.Log("Player Health: " + health);
            }
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
    }
}
