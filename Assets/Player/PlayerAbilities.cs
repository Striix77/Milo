using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Melee Settings")]
    public float startTimeBtwMelees;
    private float timeBtwMelees = 0;
    public Transform meleePos;
    public LayerMask whatIsEnemy;
    public float meleeRange;
    public int damage;
    public float verticalKnockbackForce;
    public float horizontalKnockbackForce;
    public static bool isMeleeing = false;

    [Header("Ability1 Settings")]
    public float startTimeBtwAbility1;
    private float timeBtwAbility1 = 0;
    public Transform ability1Pos;
    public float ability1Range;
    public int ability1Damage;
    public float ability1VerticalKnockbackForce;
    public float ability1HorizontalKnockbackForce;
    public static bool isAbility1ing = false;
    public GameObject ability1;
    public ParticleSystem ability1Effect;

    [Header("Ultimate Settings")]
    public float startTimeBtwUltimate;
    private float timeBtwUltimate = 0;
    public Transform ultimatePos;
    public float ultimateRange;
    public int ultimateDamage;
    public float ultimateVerticalKnockbackForce;
    public float ultimateHorizontalKnockbackForce;
    public bool isUlting = false;
    public GameObject ultimate;
    private GameObject ultimateClone;
    public ParticleSystem ultimateEffect;

    public CinemachineVirtualCamera vCam;
    public float shakeIntensity;
    public float shakeTime;

    private float normalCameraSize = 5f;
    [Range(1f, 10f)] public float zoomedOutSize = 8f;
    [Range(0.1f, 3f)] public float zoomDuration = 0.5f;
    private bool isZoomedOut = false;
    private float currentZoomVelocity = 0f;


    public RectTransform upperBar;
    public RectTransform lowerBar;
    [Range(0f, 1000f)] public float barYPosition;
    [Range(0.1f, 3f)] public float barLerpSpeed = 3f;


    public CinemachineBrain cinemachineBrain;
    private new Camera camera;
    private Bounds cameraBounds;
    private HurricaneAnimator hurricaneAnimator;

    [Range(0.1f, 5f)] public float StartLerpSpeed;
    [Range(0.1f, 5f)] public float AttackSmoothTime;
    private int phase = 0;
    private float velocity = 1f;
    private float posX1;
    private float posX2;
    private float posX3;
    private float posX4;

    void Start()
    {
        if (cinemachineBrain != null)
        {
            camera = cinemachineBrain.OutputCamera;
        }
        else
        {
            Debug.LogError("No CinemachineBrain found in scene!");
        }
    }

    void Update()
    {
        CheckForMelee();
        CheckForAbility1();
        CheckForUltimate();
        MoveBarsWhenUlting();

        UpdateCameraZoom();
        Debug.Log(camera);
        float cameraHeight = 2f * camera.orthographicSize;
        float cameraWidth = cameraHeight * camera.aspect;
        cameraBounds = new Bounds(camera.transform.position, new Vector3(cameraWidth, cameraHeight, 0));
        Debug.Log(cameraBounds.size.x);
        Vector3 cameraCenter = cameraBounds.center;

        posX1 = cameraCenter.x + cameraBounds.extents.x * 0.5f;
        posX2 = cameraCenter.x - cameraBounds.extents.x * 0.5f;
        posX3 = cameraCenter.x + cameraBounds.extents.x * 0.75f;
        posX4 = cameraCenter.x - cameraBounds.extents.x - 5f;
        LerpToStartLocation();
        LerpAroundTheCamera();

    }

    void CheckForMelee()
    {
        if (timeBtwMelees <= 0)
        {
            isMeleeing = false;
            if (InputManager.MeleeWasPressed && !isUlting)
            {
                isMeleeing = true;
                Collider2D[] enemiesToMelee = Physics2D.OverlapCircleAll(meleePos.position, meleeRange, whatIsEnemy);
                for (int i = 0; i < enemiesToMelee.Length; i++)
                {
                    enemiesToMelee[i].GetComponent<Enemy>().TakeDamage(damage);
                    Vector2 horizontalKnockback = enemiesToMelee[i].transform.position - transform.position;
                    enemiesToMelee[i].GetComponent<Rigidbody2D>().AddForce(horizontalKnockback.normalized * horizontalKnockbackForce + new Vector2(0, 1) * verticalKnockbackForce, ForceMode2D.Impulse);
                }
                timeBtwMelees = startTimeBtwMelees;
            }

        }
        else
        {
            timeBtwMelees -= Time.deltaTime;
        }


    }

    void CheckForUltimate()
    {
        if (ultimateClone == null)
        {
            isUlting = false;
        }

        if (timeBtwUltimate <= 0)
        {
            isUlting = false;
            if (InputManager.UltimateWasPressed)
            {
                isUlting = true;
                phase = 0;
                velocity = 1f;
                ultimateClone = Instantiate(ultimate, ultimatePos.position, Quaternion.identity);
                hurricaneAnimator = ultimateClone.GetComponent<HurricaneAnimator>();

                timeBtwUltimate = startTimeBtwUltimate;
            }

        }
        else
        {
            timeBtwUltimate -= Time.deltaTime;
        }


    }

    private void LerpToStartLocation()
    {
        if (ultimateClone == null || hurricaneAnimator == null)
        {
            return;
        }

        if (hurricaneAnimator.canDamage)
        {
            return;
        }
        Debug.Log("Lerping to start location");
        Vector3 newPosition = new Vector3(
            Mathf.Lerp(ultimateClone.transform.position.x, cameraBounds.center.x, StartLerpSpeed * Time.deltaTime),
            Mathf.Lerp(ultimateClone.transform.position.y, cameraBounds.center.y + 2, StartLerpSpeed * Time.deltaTime),
            ultimateClone.transform.position.z
        );

        ultimateClone.transform.position = newPosition;
        if (ultimateClone.transform.position.x >= cameraBounds.center.x - 0.1f)
        {
            phase = 1;
        }
    }

    private void LerpAroundTheCamera()
    {
        if (hurricaneAnimator == null || !hurricaneAnimator.canDamage || ultimateClone == null)
        {
            return;
        }

        Vector3 newPosition = ultimateClone.transform.position;
        switch (phase)
        {
            case 1:
                newPosition = new Vector3(
                    Mathf.SmoothDamp(ultimateClone.transform.position.x, posX1, ref velocity, AttackSmoothTime),
                    ultimateClone.transform.position.y,
                    ultimateClone.transform.position.z
                );

                if (ultimateClone.transform.position.x >= posX1 - 0.1f)
                {
                    phase = 2;
                }
                break;
            case 2:
                newPosition = new Vector3(
                    Mathf.SmoothDamp(ultimateClone.transform.position.x, posX2, ref velocity, AttackSmoothTime),
                    ultimateClone.transform.position.y,
                    ultimateClone.transform.position.z
                );

                if (ultimateClone.transform.position.x <= posX2 + 0.1f)
                {
                    phase = 3;
                }
                break;
            case 3:
                newPosition = new Vector3(
                    Mathf.SmoothDamp(ultimateClone.transform.position.x, posX3, ref velocity, AttackSmoothTime),
                    ultimateClone.transform.position.y,
                    ultimateClone.transform.position.z
                );

                if (ultimateClone.transform.position.x >= posX3 - 0.1f)
                {
                    phase = 4;
                    hurricaneAnimator.StopHurricane();
                }
                break;
            case 4:
                newPosition = new Vector3(
                    Mathf.SmoothDamp(ultimateClone.transform.position.x, posX4, ref velocity, AttackSmoothTime),
                    ultimateClone.transform.position.y,
                    ultimateClone.transform.position.z
                );

                if (ultimateClone.transform.position.x <= posX4 + 0.1f)
                {
                    phase = 0;
                }
                break;
        }

        ultimateClone.transform.position = newPosition;
    }

    private void UpdateCameraZoom()
    {
        if (isUlting && !isZoomedOut)
        {
            ZoomCamera(zoomedOutSize, ref isZoomedOut);
        }
        else if (!isUlting && isZoomedOut)
        {
            ZoomCamera(normalCameraSize, ref isZoomedOut);
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


    void CheckForAbility1()
    {
        if (timeBtwAbility1 <= 0)
        {
            isAbility1ing = false;
            if (InputManager.Ability1WasPressed && !isUlting)
            {
                isAbility1ing = true;
                GameObject ability1Clone = Instantiate(ability1, ability1Pos.position, Quaternion.identity);
                ParticleSystem ability1EffectClone = Instantiate(ability1Effect, ability1Pos.position, Quaternion.identity);
                ability1EffectClone.Play();
                Collider2D[] enemiesToAbility1 = Physics2D.OverlapCircleAll(ability1Pos.position, ability1Range, whatIsEnemy);
                for (int i = 0; i < enemiesToAbility1.Length; i++)
                {
                    enemiesToAbility1[i].GetComponent<Enemy>().TakeDamage(damage);
                    Vector2 horizontalKnockback = enemiesToAbility1[i].transform.position - transform.position;
                    enemiesToAbility1[i].GetComponent<Rigidbody2D>().AddForce(horizontalKnockback.normalized * ability1HorizontalKnockbackForce + new Vector2(0, 1) * ability1VerticalKnockbackForce, ForceMode2D.Impulse);
                }
                timeBtwAbility1 = startTimeBtwAbility1;
            }

        }
        else
        {
            timeBtwAbility1 -= Time.deltaTime;
        }


    }


    private void MoveBarsWhenUlting()
    {
        if (isUlting)
        {
            Debug.Log("Sziasztok");
            upperBar.anchoredPosition = Vector2.Lerp(upperBar.anchoredPosition, new Vector2(upperBar.anchoredPosition.x, barYPosition), barLerpSpeed * Time.deltaTime);
            lowerBar.anchoredPosition = Vector2.Lerp(lowerBar.anchoredPosition, new Vector2(lowerBar.anchoredPosition.x, -barYPosition), barLerpSpeed * Time.deltaTime);
        }
        else
        {
            upperBar.anchoredPosition = Vector2.Lerp(upperBar.anchoredPosition, new Vector2(upperBar.anchoredPosition.x, 645), barLerpSpeed * Time.deltaTime);
            lowerBar.anchoredPosition = Vector2.Lerp(lowerBar.anchoredPosition, new Vector2(lowerBar.anchoredPosition.x, -645), barLerpSpeed * Time.deltaTime);
        }
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(meleePos.position, meleeRange);

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(ability1Pos.position, ability1Range);

    }
}
