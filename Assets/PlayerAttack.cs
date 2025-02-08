using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBtwMelees = 0;
    public float startTimeBtwMelees;
    public Transform meleePos;
    public LayerMask whatIsEnemy;
    public float meleeRange;
    public int damage;
    public float verticalKnockbackForce;
    public float horizontalKnockbackForce;

    public CinemachineVirtualCamera vCam;
    public float shakeIntensity;
    public float shakeTime;

    private float timer;
    private CinemachineBasicMultiChannelPerlin cmbcp;


    void Update()
    {

        CheckForMelee();

    }

    void CheckForMelee()
    {
        if (timeBtwMelees <= 0)
        {
            if (Input.GetKey(KeyCode.E))
            {
                Collider2D[] enemiesToMelee = Physics2D.OverlapCircleAll(meleePos.position, meleeRange, whatIsEnemy);
                for (int i = 0; i < enemiesToMelee.Length; i++)
                {
                    enemiesToMelee[i].GetComponent<Enemy>().TakeDamage(damage);
                    Vector2 horizontalKnockback = enemiesToMelee[i].transform.position - transform.position;
                    enemiesToMelee[i].GetComponent<Rigidbody2D>().AddForce(horizontalKnockback.normalized * horizontalKnockbackForce + new Vector2(0, 1) * verticalKnockbackForce, ForceMode2D.Impulse);
                }
                timeBtwMelees = startTimeBtwMelees;
                if (enemiesToMelee.Length > 0)
                {
                    ShakeCamera();
                }
            }

        }
        else
        {
            timeBtwMelees -= Time.deltaTime;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StopShake();
            }
        }
    }

    void ShakeCamera()
    {
        cmbcp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cmbcp.m_AmplitudeGain = shakeIntensity;

        timer = shakeTime;
    }

    void StopShake()
    {
        cmbcp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cmbcp.m_AmplitudeGain = 0;

        timer = 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(meleePos.position, meleeRange);

    }
}
