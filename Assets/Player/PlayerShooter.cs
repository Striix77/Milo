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

    private float timeBtwFire;
    public float startTimeBtwFire;

    void Update()
    {
        HandleRotation();
        if (timeBtwFire <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                GameObject boom = Instantiate(PeaBoom, attackPos.position, Quaternion.identity);
                Instantiate(shadowAttack1, attackPos.position, Quaternion.identity);
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
