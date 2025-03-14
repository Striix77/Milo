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
    public static bool isMeleeing = false;

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
            isMeleeing = false;
            if (InputManager.MeleeWasPressed)
            {
                isMeleeing = true;
                Debug.Log("Melee");
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

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // StopShake();
            }
        }
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(meleePos.position, meleeRange);

    }
}
