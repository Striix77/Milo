using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 1f;
    public bool spawnPrefabWhenHit = false;
    public GameObject prefabToSpawnOnHit;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject player = collision.gameObject.transform.root.gameObject;
            player.GetComponent<PlayerStatusManager>().TakeDamage(damage);
            if (spawnPrefabWhenHit && prefabToSpawnOnHit != null)
            {
                GameObject spawnedPrefab = Instantiate(prefabToSpawnOnHit, transform.position, Quaternion.identity);
                Destroy(spawnedPrefab, 0.3f);
            }
            Destroy(gameObject);
        }
    }

 


}
