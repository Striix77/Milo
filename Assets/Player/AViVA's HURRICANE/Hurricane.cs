using System.Collections;
using UnityEngine;

public class Hurricane : MonoBehaviour
{
    [Range(0.1f, 5f)] public float StartLerpSpeed;
    [Range(0.1f, 5f)] public float AttackSmoothTime;

    private HurricaneAnimator hurricaneAnimator;

    private Camera camera;
    private Bounds cameraBounds;

    private float posX1;
    private float posX2;
    private float posX3;
    private float posX4;

    private static int phase = 0;
    private float velocity = 1f;

    


    void Start()
    {
        camera = Camera.main;
        float cameraHeight = 2f * camera.orthographicSize;
        float cameraWidth = cameraHeight * camera.aspect;
        cameraBounds = new Bounds(camera.transform.position, new Vector3(cameraWidth, cameraHeight, 0));
        posX1 = (cameraBounds.center.x + cameraBounds.size.x / 2) / 2;
        posX2 = (cameraBounds.center.x - cameraBounds.size.x / 2) / 2;
        posX3 = (cameraBounds.center.x + cameraBounds.size.x / 2) / 1.5f;
        posX4 = cameraBounds.center.x - cameraBounds.size.x / 2 - 5;
        hurricaneAnimator = GetComponent<HurricaneAnimator>();
    }

    void Update()
    {
        float cameraHeight = 2f * camera.orthographicSize;
        float cameraWidth = cameraHeight * camera.aspect;
        cameraBounds = new Bounds(camera.transform.position, new Vector3(cameraWidth, cameraHeight, 0));
        posX1 = (cameraBounds.center.x + cameraBounds.size.x / 2) / 2;
        posX2 = (cameraBounds.center.x - cameraBounds.size.x / 2) / 2;
        posX3 = (cameraBounds.center.x + cameraBounds.size.x / 2) / 1.5f;
        posX4 = cameraBounds.center.x - cameraBounds.size.x / 2 - 5;
        LerpToStartLocation();
        LerpAroundTheCamera();

        
    }


    private void LerpToStartLocation()
    {
        Debug.Log(hurricaneAnimator.canDamage);
        if (hurricaneAnimator.canDamage)
        {
            return;
        }
        Vector3 newPosition = new Vector3(
            Mathf.Lerp(transform.position.x, cameraBounds.center.x, StartLerpSpeed * Time.deltaTime),
            Mathf.Lerp(transform.position.y, cameraBounds.center.y + 2, StartLerpSpeed * Time.deltaTime),
            transform.position.z
        );

        transform.position = newPosition;
        if (transform.position.x >= cameraBounds.center.x - 0.1f)
        {
            phase = 1;
        }
    }

    private void LerpAroundTheCamera()
    {
        if (!hurricaneAnimator.canDamage)
        {
            return;
        }

        Vector3 newPosition = transform.position;
        switch (phase)
        {
            case 1:
                newPosition = new Vector3(
                    Mathf.SmoothDamp(transform.position.x, posX1, ref velocity, AttackSmoothTime),
                    transform.position.y,
                    transform.position.z
                );

                if (transform.position.x >= posX1 - 0.1f)
                {
                    phase = 2;
                }
                break;
            case 2:
                newPosition = new Vector3(
                    Mathf.SmoothDamp(transform.position.x, posX2, ref velocity, AttackSmoothTime),
                    transform.position.y,
                    transform.position.z
                );

                if (transform.position.x <= posX2 + 0.1f)
                {
                    phase = 3;
                }
                break;
            case 3:
                newPosition = new Vector3(
                    Mathf.SmoothDamp(transform.position.x, posX3, ref velocity, AttackSmoothTime),
                    transform.position.y,
                    transform.position.z
                );

                if (transform.position.x >= posX3 - 0.1f)
                {
                    phase = 4;
                    hurricaneAnimator.StopHurricane();
                }
                break;
            case 4:
                newPosition = new Vector3(
                    Mathf.SmoothDamp(transform.position.x, posX4, ref velocity, AttackSmoothTime),
                    transform.position.y,
                    transform.position.z
                );

                if (transform.position.x <= posX4 + 0.1f)
                {
                    phase = 0;
                }
                break;
        }

        transform.position = newPosition;
    }
}
