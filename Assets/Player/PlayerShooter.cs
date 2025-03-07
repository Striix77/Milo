using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Camera mainCamera;
    private Vector3 mousePos;
    public Transform playerPos;
    public Transform attackPos;
    public GameObject shadowAttack1;
    public GameObject PeaBoom;
    public LineRenderer lineRenderer;
    private Vector3[] SegmentPos;

    private float timeBtwFire;
    public float startTimeBtwFire;


    void Update()
    {
        HandleRotation();
        if (timeBtwFire <= 0)
        {
            if (Input.GetMouseButton(0))
            {

                if (SegmentPos == null || SegmentPos.Length != lineRenderer.positionCount)
                {
                    SegmentPos = new Vector3[lineRenderer.positionCount];
                    Debug.Log(SegmentPos);
                }

                lineRenderer.GetPositions(SegmentPos);
                Vector3 spawnPos = SegmentPos[SegmentPos.Length - 1];
                GameObject boom = Instantiate(PeaBoom, spawnPos, Quaternion.identity);
                Instantiate(shadowAttack1, spawnPos, Quaternion.identity);
                timeBtwFire = startTimeBtwFire;
                Destroy(boom, 0.3f);
            }
        }
        else
        {
            timeBtwFire -= Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        transform.position = playerPos.position;
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
