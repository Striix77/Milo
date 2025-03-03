using UnityEngine;

public class Ghosting : MonoBehaviour
{
    [Range(0.01f, 1f)] public float ghostDelay = 0.1f;
    [Range(0.1f, 2f)] public float ghostLife = 1f;
    private float ghostDelayTimer;
    private SpriteRenderer ghostRenderer;
    public GameObject ghostPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ghostDelayTimer = ghostDelay;
        ghostRenderer = ghostPrefab.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMovementStats.createGhost)
        {
            CreateGhost();
        }

    }

    private void CreateGhost()
    {
        if (ghostDelayTimer > 0)
        {
            ghostDelayTimer -= Time.deltaTime;
        }
        else
        {
            ghostDelayTimer = ghostDelay;
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            ghostRenderer.sprite = GetComponent<SpriteRenderer>().sprite;

            ghost.transform.rotation = transform.rotation;
            Destroy(ghost, ghostLife);
        }
    }
}
