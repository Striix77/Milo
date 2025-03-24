using System.Collections;
using UnityEngine;

public class Hurricane : MonoBehaviour
{
    

    private HurricaneAnimator hurricaneAnimator;

    private static int phase = 0;
    private float velocity = 1f;



    void Start()
    {
        hurricaneAnimator = GetComponent<HurricaneAnimator>();
    }

    void Update()
    {

    }



}
