using UnityEngine;

public enum StorkState { MovingRight, MovingLeft, WaitingRight, WaitingLeft }

public class StorkMovement : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Movement Settings")]
    public float horizontalSpeed = 300f;
    public float verticalOffset = 3f;
    public float waitTime = 3f;

    private float spriteWidth;
    private float waitTimer = 0f;

    private StorkState currentState = StorkState.WaitingLeft;
    private int storkBombAnimation = Animator.StringToHash("StorkWithABomb");
    private Animator animator;

    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteWidth = spriteRenderer.bounds.size.x;
        }
        else
        {
            spriteWidth = 1f;
        }

        PositionAtLeftEdge();
    }

    private void PositionAtLeftEdge()
    {
        Vector2 screenBounds = GetCurrentScreenBounds();

        float leftEdge = mainCamera.transform.position.x - screenBounds.x - spriteWidth;
        float yPosition = mainCamera.transform.position.y + verticalOffset;

        transform.position = new Vector3(leftEdge, yPosition, 0);

        transform.localScale = new Vector3(-3, 3, 3);
    }

    void Update()
    {
        MoveHorizontally();
    }

    void MoveHorizontally()
    {
        Vector2 screenBounds = GetCurrentScreenBounds();

        float leftEdge = mainCamera.transform.position.x - screenBounds.x - spriteWidth;
        float rightEdge = mainCamera.transform.position.x + screenBounds.x + spriteWidth;

        Vector3 currentPosition = transform.position;
        float yPosition = mainCamera.transform.position.y + verticalOffset;

        switch (currentState)
        {
            case StorkState.MovingRight:
                currentPosition.x += horizontalSpeed * Time.deltaTime;
                if (currentPosition.x >= rightEdge)
                {
                    currentPosition.x = rightEdge;
                    currentState = StorkState.WaitingRight;
                    waitTimer = 0f;
                }
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<Collider2D>().enabled = true;
                break;

            case StorkState.MovingLeft:
                currentPosition.x -= horizontalSpeed * Time.deltaTime;
                if (currentPosition.x <= leftEdge)
                {
                    currentPosition.x = leftEdge;
                    currentState = StorkState.WaitingLeft;
                    waitTimer = 0f;
                }
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<Collider2D>().enabled = true;
                break;

            case StorkState.WaitingRight:
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTime)
                {
                    currentState = StorkState.MovingLeft;
                }
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
                animator.CrossFade(storkBombAnimation, 0.1f, 0);
                break;

            case StorkState.WaitingLeft:
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTime)
                {
                    currentState = StorkState.MovingRight;
                }
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
                animator.CrossFade(storkBombAnimation, 0.1f, 0);
                break;
        }

        currentPosition.y = yPosition;

        transform.position = currentPosition;

        bool isMovingRight = currentState.Equals(StorkState.MovingRight);
        transform.localScale = new Vector3(isMovingRight ? -3 : 3, 3, 3);
    }

    Vector2 GetCurrentScreenBounds()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        return new Vector2(cameraWidth / 2, cameraHeight / 2);
    }
}
