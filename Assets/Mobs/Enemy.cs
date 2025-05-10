using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public GameObject DeathSmokePrefab;
    private WaveManager waveManager;

    void Start()
    {
        waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
    }

    void DestroyEnemy()
    {
        waveManager.RemoveEnemy(gameObject);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        GameObject smoke = Instantiate(DeathSmokePrefab, transform.position, Quaternion.identity);
        Destroy(smoke, 0.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, bool changeColor = true)
    {
        health -= damage;
        Debug.Log(health);
        if (changeColor)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            Invoke(nameof(ResetColor), 0.1f);
        }
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.1f);
        }
    }

    public void Freeze(float freezeTime)
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.3981131f, 1f, 0.955977f);
        MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script is Enemy)
                continue;
            script.enabled = false;
        }
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Invoke(nameof(Unfreeze), freezeTime);
    }

    public void Unfreeze()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
        gameObject.GetComponent<Collider2D>().enabled = true;
        gameObject.GetComponent<Animator>().enabled = true;
        if (gameObject.name.Contains("Diggly"))
        {
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void ResetColor()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
