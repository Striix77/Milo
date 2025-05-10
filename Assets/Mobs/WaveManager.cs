using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public EnemyWave currentWave;

    public float timeBetweenWaves = 2f;
    private int waveCounter = 0;
    private bool waveInProgress = false;
    private float waveCooldown = 0f;
    private Camera mainCamera;
    private List<GameObject> enemiesAlive;

    void Start()
    {
        mainCamera = Camera.main;
        enemiesAlive = new List<GameObject>();
    }

    void Update()
    {
        if (!waveInProgress)
        {
            if (waveCooldown <= 0f)
            {
                StartWave();
            }
            else
            {
                waveCooldown -= Time.deltaTime;
            }
        }

    }

    void StartWave()
    {
        Debug.Log($"Starting wave {waveCounter + 1}");
        currentWave = GenerateNewWave();

        for (int i = 0; i < currentWave.enemiesToSpawn.Count; i++)
        {
            Vector2 screenBounds = GetCurrentScreenBounds();
            float leftEdge = mainCamera.transform.position.x - screenBounds.x - 2;
            float rightEdge = mainCamera.transform.position.x + screenBounds.x + 2;
            Vector3 spawnPos = Random.Range(0, 2) == 0 ? new Vector3(leftEdge, Random.Range(0, 5), 0) : new Vector3(rightEdge, Random.Range(0, 5), 0);
            GameObject enemy = Instantiate(currentWave.enemiesToSpawn[i], spawnPos, Quaternion.identity);
            enemiesAlive.Add(enemy);
        }

        waveInProgress = true;
        waveCounter++;
    }

    public EnemyWave GenerateNewWave()
    {
        EnemyWave newWave = Resources.Load<EnemyWave>("Waves/DefaultEnemyWave");
        if ((waveCounter + 1) % 5 == 0)
        {
            newWave.digglyCost += newWave.digglyCost * newWave.costModifier / 100;
            newWave.storkCost += newWave.storkCost * newWave.costModifier / 100;
            newWave.sonnyCost += newWave.sonnyCost * newWave.costModifier / 100;
            newWave.digglyPoints += newWave.digglyPoints * newWave.pointsModifier / 100;
            newWave.storkPoints += newWave.storkPoints * newWave.pointsModifier / 100;
            newWave.sonnyPoints += newWave.sonnyPoints * newWave.pointsModifier / 100;

            newWave.digglyPrefab.GetComponent<Enemy>().health += newWave.digglyPrefab.GetComponent<Enemy>().health * newWave.healthModifier / 100;
            newWave.storkPrefab.GetComponent<Enemy>().health += newWave.storkPrefab.GetComponent<Enemy>().health * newWave.healthModifier / 100;
            newWave.sonnyPrefab.GetComponent<Enemy>().health += newWave.sonnyPrefab.GetComponent<Enemy>().health * newWave.healthModifier / 100;

            newWave.digglyPrefab.GetComponent<EnemyProjectile>().damage += newWave.digglyPrefab.GetComponent<EnemyProjectile>().damage * newWave.dmgModifier / 100;
            newWave.storkPrefab.GetComponent<EnemyProjectile>().damage += newWave.storkPrefab.GetComponent<EnemyProjectile>().damage * newWave.dmgModifier / 100;
            newWave.sonnyPrefab.GetComponent<EnemyProjectile>().damage += newWave.sonnyPrefab.GetComponent<EnemyProjectile>().damage * newWave.dmgModifier / 100;


        }
        if (currentWave != null)
        {
            newWave.waveValue += currentWave.waveValue;
        }
        while (newWave.waveValue >= newWave.digglyCost || newWave.waveValue >= newWave.storkCost || newWave.waveValue >= newWave.sonnyCost)
        {
            int randomEnemyIndex = Random.Range(0, 3);
            switch (randomEnemyIndex)
            {
                case 0:
                    newWave.enemiesToSpawn.Add(newWave.digglyPrefab);
                    newWave.waveValue -= newWave.digglyCost;
                    break;
                case 1:
                    newWave.enemiesToSpawn.Add(newWave.storkPrefab);
                    newWave.waveValue -= newWave.storkCost;
                    break;
                case 2:
                    newWave.enemiesToSpawn.Add(newWave.sonnyPrefab);
                    newWave.waveValue -= newWave.sonnyCost;
                    break;
            }
        }
        return newWave;
    }

    Vector2 GetCurrentScreenBounds()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        return new Vector2(cameraWidth / 2, cameraHeight / 2);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemiesAlive.Remove(enemy);
        if (enemiesAlive.Count == 0)
        {
            waveInProgress = false;
            waveCooldown = timeBetweenWaves;
            // currentWave.enemiesToSpawn.Clear();
        }
    }
}
