using UnityEngine;

public class ShadowAttack1 : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mousePos;
    public Rigidbody2D rb;
    public float force;
    public int damage;
    public float horizontalKnockbackForce;
    public float verticalKnockbackForce;
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ + 180);
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
        Invoke(nameof(DestroyAttack), 5);
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Enemy"))
        {
            coll.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Vector2 horizontalKnockback = coll.gameObject.transform.position - transform.position;
            coll.gameObject.GetComponent<Rigidbody2D>().AddForce(horizontalKnockback.normalized * horizontalKnockbackForce + new Vector2(0, 1) * verticalKnockbackForce, ForceMode2D.Impulse);
            DestroyAttack();
        }
        else if (coll.gameObject.CompareTag("Ground"))
        {
            DestroyAttack();
        }
    }

    void DestroyAttack()
    {
        Destroy(gameObject);
    }
}
