using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Hurricane : MonoBehaviour
{


    private HurricaneAnimator hurricaneAnimator;
    private BoxCollider2D hurricaneCollider;
    public GameObject AttractionPoint;
    public float AttractionForce;
    private float attractionForce;
    public int Damage;
    public float TimeBetweenDamage;
    public float TimeUntillFullForce;
    private float dmgTimer = 0;
    private float forceTimer = 0;



    void Start()
    {
        hurricaneAnimator = GetComponent<HurricaneAnimator>();
        hurricaneCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (hurricaneAnimator.canDamage)
        {
            AttractAndDamageEnemies();
        }


    }

    private void AttractAndDamageEnemies()
    {
        dmgTimer += Time.deltaTime;
        forceTimer += Time.deltaTime;
        if (hurricaneCollider != null)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(
                transform.position,
                hurricaneCollider.size,
                transform.rotation.eulerAngles.z
            );

            foreach (Collider2D col in colliders)
            {
                if (col.gameObject.CompareTag("Enemy"))
                {
                    Vector2 direction = AttractionPoint.transform.position - col.transform.position;
                    Rigidbody2D enemyRb = col.GetComponent<Rigidbody2D>();
                    attractionForce = Mathf.Lerp(0, AttractionForce, forceTimer / TimeUntillFullForce);
                    if (enemyRb != null)
                    {
                        enemyRb.AddForce(direction.normalized * attractionForce, ForceMode2D.Force);
                    }

                    if (dmgTimer >= TimeBetweenDamage)
                    {
                        Enemy enemy = col.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(Damage);
                        }
                    }
                }
            }

            if (dmgTimer >= TimeBetweenDamage)
            {
                dmgTimer = 0;
            }
        }
    }


}


