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

    private float timeBtwAbility1 = 0;
    public float startTimeBtwAbility1;
    public Transform ability1Pos;
    public float ability1Range;
    public int ability1Damage;
    public float ability1VerticalKnockbackForce;
    public float ability1HorizontalKnockbackForce;
    public static bool isAbility1ing = false;
    public GameObject ability1;
    public ParticleSystem ability1Effect;

    public CinemachineVirtualCamera vCam;
    public float shakeIntensity;
    public float shakeTime;

    private CinemachineBasicMultiChannelPerlin cmbcp;


    void Update()
    {

        CheckForMelee();
        CheckForAbility1();
    }

    void CheckForMelee()
    {
        if (timeBtwMelees <= 0)
        {
            isMeleeing = false;
            if (InputManager.MeleeWasPressed)
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

    void CheckForAbility1()
    {
        if (timeBtwAbility1 <= 0)
        {
            isAbility1ing = false;
            if (InputManager.Ability1WasPressed)
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



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(meleePos.position, meleeRange);

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(ability1Pos.position, ability1Range);

    }
}
