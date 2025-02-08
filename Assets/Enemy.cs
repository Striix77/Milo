using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
    }
}
