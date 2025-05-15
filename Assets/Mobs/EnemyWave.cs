using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWave", menuName = "Scriptable Objects/EnemyWave")]
public class EnemyWave : ScriptableObject
{
    public GameObject digglyPrefab;
    public GameObject storkPrefab;
    public GameObject sonnyPrefab;
    public int waveValue = 30;
    public int digglyCost = 7;
    public int storkCost = 10;
    public int sonnyCost = 15;
    public float spawnInterval = 2f;
    public int digglyPoints = 10;
    public int storkPoints = 15;
    public int sonnyPoints = 25;
    public int waveValueModifier = 15;
    public int costModifier = 1;
    public int pointsModifier = 1;
    public int dmgModifier = 1;
    public int healthModifier = 1;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
}
