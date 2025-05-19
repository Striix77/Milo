using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public GameObject DeathSmokePrefab;
    private WaveManager waveManager;
    private SpriteRenderer spriteRenderer;
    private SkillTreeManager skillTreeManager;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
        skillTreeManager = GameObject.Find("Skill Tree").GetComponent<SkillTreeManager>();
    }

    void DestroyEnemy()
    {
        waveManager.RemoveEnemy(gameObject);
        spriteRenderer.enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        GameObject smoke = Instantiate(DeathSmokePrefab, transform.position, Quaternion.identity);
        Destroy(smoke, 0.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, bool changeColor = true)
    {
        health -= skillTreeManager.ApplyDmgBuff(damage);
        Debug.Log(health);
        if (changeColor)
        {
            spriteRenderer.color = Color.red;
            Invoke(nameof(ResetColor), 0.1f);
        }
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.1f);
        }
    }

    public void Freeze(float freezeTime)
    {
        spriteRenderer.color = new Color(0.3981131f, 1f, 0.955977f);
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
        spriteRenderer.color = Color.white;
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
        spriteRenderer.color = Color.white;
    }
}
