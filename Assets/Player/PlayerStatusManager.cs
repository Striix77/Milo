using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour
{
    public float maxHealth = 350f;
    public Slider healthSlider;
    public float health = 350f;
    public float damageShakeDuration = 0.1f;
    private float damageShakeTimer = 0f;

    [Range(0f, 5f)] public float healthLerpSpeed = 2f;
    public float targetHealth;

    private PlayerAbilities playerAbilities;
    private SpriteRenderer spriteRenderer;
    public CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin vCamNoise;
    private float shakeVelocity = 1f;
    [Range(0f, 5f)] public float shakeIntensity = 1f;
    [Range(0f, 5f)] public float ShakeStopSpeed = 1f;

    private bool isDead = false;
    public Image deathScreenImage;
    [Range(0f, 1f)] public float deathScreenEndOpacity = 0.5f;
    [Range(1f, 10f)] public float zoomedInSize = 3f;
    [Range(0.1f, 3f)] public float zoomDuration = 0.5f;
    private bool isZoomedOut = false;
    private float currentZoomVelocity = 0f;
    private Collider2D[] playerColliders;
    private PlayerMovement playerMovementScript;
    private PlayerAnimator playerAnimator;
    public PlayerShooter playerShooterScript;
    private Rigidbody2D rb;
    public GameObject HUD;
    public GameObject SkillTreeUI;
    public GameObject SkillTreeOverlay;
    public GameObject GameOverScreen;

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

        playerColliders = GetComponentsInChildren<Collider2D>();
        playerMovementScript = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<PlayerAnimator>();
        rb = GetComponent<Rigidbody2D>();
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
        if (damageShakeTimer <= damageShakeDuration)
        {
            StopShakeSmoothly();
            damageShakeTimer += Time.deltaTime;
        }
        else if (damageShakeTimer <= damageShakeDuration + 0.5f)
        {
            StopShake();
            damageShakeTimer += Time.deltaTime;
        }

        if (isDead)
        {
            ZoomCamera(zoomedInSize, ref isZoomedOut);
            IncreaseImageOpacity(deathScreenEndOpacity);
        }

        if (transform.position.y < -30)
        {
            Die();
        }
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
        GameOverScreen.SetActive(true);
        HUD.SetActive(false);
        SkillTreeUI.SetActive(false);
        SkillTreeOverlay.SetActive(false);
        isDead = true;
        deathScreenImage.gameObject.SetActive(true);
        playerMovementScript.enabled = false;
        playerAbilities.enabled = false;
        playerAnimator.enabled = false;
        playerShooterScript.enabled = false;
        foreach (Collider2D collider in playerColliders)
        {
            collider.enabled = false;
        }
        rb.linearVelocity = Vector2.zero;
        rb.mass = 0.005f;
        rb.gravityScale = 0.005f;
        rb.freezeRotation = false;
    }

    void ResetColor()
    {
        spriteRenderer.color = Color.white;
    }

    private void ShakeCamera(float intensity)
    {
        if (vCam == null)
        {
            Debug.LogWarning("Virtual Camera not assigned.");
            return;
        }
        if (vCamNoise == null)
        {
            Debug.LogWarning("CinemachineBasicMultiChannelPerlin not found on the virtual camera.");
        }
        vCamNoise.m_AmplitudeGain = intensity;
        damageShakeTimer = 0f;
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

    private void ZoomCamera(float targetSize, ref bool isZoomedOut)
    {
        float startSize = vCam.m_Lens.OrthographicSize;

        vCam.m_Lens.OrthographicSize = Mathf.SmoothDamp(
            startSize,
            targetSize,
            ref currentZoomVelocity,
            zoomDuration
        );


        if (Mathf.Abs(vCam.m_Lens.OrthographicSize - targetSize) < 0.01f)
        {
            vCam.m_Lens.OrthographicSize = targetSize;
            isZoomedOut = !isZoomedOut;
        }
    }

    private void IncreaseImageOpacity(float targetAlpha)
    {
        Color color = deathScreenImage.color;
        color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime);
        deathScreenImage.color = color;

        if (Mathf.Abs(deathScreenImage.color.a - targetAlpha) < 0.01f)
        {
            deathScreenImage.color = new Color(color.r, color.g, color.b, targetAlpha);
        }
    }

    private void StopShake()
    {
        if (vCam == null)
        {
            return;
        }

        vCamNoise.m_AmplitudeGain = 0f;
        vCam.transform.rotation = Quaternion.identity;
        shakeVelocity = 1f;
    }


}
