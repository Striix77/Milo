using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject hurricanePrefab;
    void Start()
    {

        GameObject preWarmHurricane = Instantiate(hurricanePrefab, new Vector3(-1000, -1000, -1000), Quaternion.identity);
        Destroy(preWarmHurricane, 0.01f);
    }


    void Update()
    {

    }
}
