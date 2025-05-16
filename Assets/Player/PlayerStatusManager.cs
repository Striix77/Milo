using Cinemachine;
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
    private SpriteRenderer spriteRenderer;
    public CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin vCamNoise;
    private float velocity = 1f;
    private float shakeVelocity = 1f;
    [Range(0f, 5f)] public float shakeIntensity = 1f;
    [Range(0f, 5f)] public float ShakeStopSpeed = 1f;


    void Start()
    {
        health = maxHealth;
        targetHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        playerAbilities = GetComponent<PlayerAbilities>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        vCamNoise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
        // StopShakeSmoothly();
    }

    public void TakeDamage(float damage)
    {
        if (!playerAbilities.isUlting)
        {
            spriteRenderer.color = Color.red;
            Invoke(nameof(ResetColor), 0.1f);
            ShakeCamera(shakeIntensity);
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

    void ResetColor()
    {
        spriteRenderer.color = Color.white;
    }

    private void ShakeCamera(float intensity)
    {
        if (vCam == null)
        {
            return;
        }

        vCamNoise.m_AmplitudeGain = intensity;

    }

    private void StopShakeSmoothly()
    {
        if (vCam == null)
        {
            return;
        }

        vCamNoise.m_AmplitudeGain = Mathf.SmoothDamp(
            vCamNoise.m_AmplitudeGain,
            0f,
            ref shakeVelocity,
            ShakeStopSpeed
        );

        if (vCamNoise.m_AmplitudeGain < 0.01f)
        {
            vCamNoise.m_AmplitudeGain = 0f;
            shakeVelocity = 1f;
        }
    }

    private void StopShake()
    {
        if (vCam == null)
        {
            return;
        }

        vCamNoise.m_AmplitudeGain = 0f;
        shakeVelocity = 1f;
    }


}
