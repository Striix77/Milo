using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public EnemyWave currentWave;

    public float timeBetweenWaves = 2f;
    private int waveCounter = 0;
    private bool waveInProgress = false;
    private float waveCooldown = 0f;
    private Camera mainCamera;
    private List<GameObject> enemiesAlive;
    private int waveValue = 30;
    public Image WaveOverlay;
    public TextMeshProUGUI WaveText;
    private float waveTextAppearTime = 0.5f;
    private float waveTextAppearTimer = 0f;
    private String waveText = "Wave 1";
    private int textIndex = 0;
    private bool textPhase = true;
    public GameOver gameOverScreen;
    public int score = 0;
    public PlayerAbilities playerAbilities;
    public SkillTreeManager skillTreeManager;
    public GameObject SkillTreeOverlay;
    public GameObject SkillTreePanel;
    private bool isSkillTreeOpen = false;
    private bool skillTreeJustOpened = true;
    private int skillTreeLength = 0;

    void Awake()
    {
        waveText = "Wave 1";
        skillTreeLength = 0;
    }

    void Start()
    {
        mainCamera = Camera.main;
        enemiesAlive = new List<GameObject>();
        waveCooldown = timeBetweenWaves;
        waveTextAppearTime = timeBetweenWaves / 12;
        skillTreeManager = GameObject.Find("Skill Tree").GetComponent<SkillTreeManager>();

    }

    void Update()
    {
        if (!waveInProgress)
        {
            if (waveCooldown <= 0f && !isSkillTreeOpen)
            {
                StartWave();
            }
            else
            {
                waveCooldown -= Time.deltaTime;
            }
        }
        OnGUI();
        OnSkillTree();
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
            Vector3 spawnPos = UnityEngine.Random.Range(0, 2) == 0 ? new Vector3(leftEdge, UnityEngine.Random.Range(0, 5), 0) : new Vector3(rightEdge, UnityEngine.Random.Range(0, 5), 0);
            GameObject enemy = Instantiate(currentWave.enemiesToSpawn[i], spawnPos, Quaternion.identity);
            enemiesAlive.Add(enemy);
        }

        waveInProgress = true;
        waveCounter++;
    }

    public EnemyWave GenerateNewWave()
    {
        EnemyWave originalWave = Resources.Load<EnemyWave>("Waves/DefaultEnemyWave");
        EnemyWave newWave = Instantiate(originalWave);
        if ((waveCounter + 1) % 5 == 0)
        {
            waveValue += waveValue * newWave.waveValueModifier / 100;
            newWave.digglyCost += newWave.digglyCost * newWave.costModifier / 100;
            newWave.storkCost += newWave.storkCost * newWave.costModifier / 100;
            newWave.sonnyCost += newWave.sonnyCost * newWave.costModifier / 100;
            newWave.digglyPoints += newWave.digglyPoints * newWave.pointsModifier / 100;
            newWave.storkPoints += newWave.storkPoints * newWave.pointsModifier / 100;
            newWave.sonnyPoints += newWave.sonnyPoints * newWave.pointsModifier / 100;

            newWave.digglyPrefab.GetComponent<Enemy>().health += newWave.digglyPrefab.GetComponent<Enemy>().health * newWave.healthModifier / 100;
            newWave.storkPrefab.GetComponent<Enemy>().health += newWave.storkPrefab.GetComponent<Enemy>().health * newWave.healthModifier / 100;
            newWave.sonnyPrefab.GetComponent<Enemy>().health += newWave.sonnyPrefab.GetComponent<Enemy>().health * newWave.healthModifier / 100;

            newWave.digglyPrefab.GetComponent<DigglyAttack>().dmgModifier += newWave.digglyPrefab.GetComponent<DigglyAttack>().dmgModifier * newWave.dmgModifier / 100;
            newWave.storkPrefab.GetComponent<StorkAttack>().dmgModifier += newWave.storkPrefab.GetComponent<StorkAttack>().dmgModifier * newWave.dmgModifier / 100;
            newWave.sonnyPrefab.GetComponent<SonnyAnimator>().dmgModifier += newWave.sonnyPrefab.GetComponent<SonnyAnimator>().dmgModifier * newWave.dmgModifier / 100;


        }
        newWave.waveValue = waveValue;
        if (currentWave != null)
        {
            newWave.waveValue += currentWave.waveValue;
            newWave.digglyCost = currentWave.digglyCost;
            newWave.storkCost = currentWave.storkCost;
            newWave.sonnyCost = currentWave.sonnyCost;
            newWave.digglyPoints = currentWave.digglyPoints;
            newWave.storkPoints = currentWave.storkPoints;
            newWave.sonnyPoints = currentWave.sonnyPoints;
        }

        Debug.Log($"Wave {waveCounter + 1} - Wave Value: {newWave.waveValue} - Diggly: {newWave.digglyCost}, Stork: {newWave.storkCost}, Sonny: {newWave.sonnyCost}");
        while (newWave.waveValue >= newWave.digglyCost || newWave.waveValue >= newWave.storkCost || newWave.waveValue >= newWave.sonnyCost)
        {
            int randomEnemyIndex = UnityEngine.Random.Range(0, 3);
            switch (randomEnemyIndex)
            {
                case 0:
                    if (newWave.waveValue < newWave.digglyCost)
                    {
                        continue;
                    }
                    newWave.enemiesToSpawn.Add(newWave.digglyPrefab);
                    newWave.waveValue -= newWave.digglyCost;
                    break;
                case 1:
                    if (newWave.waveValue < newWave.storkCost)
                    {
                        continue;
                    }
                    newWave.enemiesToSpawn.Add(newWave.storkPrefab);
                    newWave.waveValue -= newWave.storkCost;
                    break;
                case 2:
                    if (newWave.waveValue < newWave.sonnyCost)
                    {
                        continue;
                    }
                    newWave.enemiesToSpawn.Add(newWave.sonnyPrefab);
                    newWave.waveValue -= newWave.sonnyCost;
                    break;
            }
            Debug.Log($"Wave {waveCounter + 1} - Wave Value: {newWave.waveValue} - Diggly: {newWave.digglyCost}, Stork: {newWave.storkCost}, Sonny: {newWave.sonnyCost}");
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
        if (enemy.name.Contains("Diggly"))
        {
            score += currentWave.digglyPoints;
            if (!playerAbilities.isUlting)
            {
                playerAbilities.timeBtwUltimate -= skillTreeManager.ApplyHurricaneBuff(currentWave.digglyPoints);
            }
        }
        else if (enemy.name.Contains("Stork"))
        {
            score += currentWave.storkPoints;
            if (!playerAbilities.isUlting)
            {
                playerAbilities.timeBtwUltimate -= skillTreeManager.ApplyHurricaneBuff(currentWave.storkPoints);
            }
        }
        else if (enemy.name.Contains("Sonny"))
        {
            score += currentWave.sonnyPoints;
            if (!playerAbilities.isUlting)
            {
                playerAbilities.timeBtwUltimate -= skillTreeManager.ApplyHurricaneBuff(currentWave.sonnyPoints);
            }
        }
        gameOverScreen.SetScore(score);
        if (enemiesAlive.Count == 0)
        {
            if ((waveCounter + 1) % 2 == 0 && skillTreeLength < 10)
            {
                isSkillTreeOpen = true;
                skillTreeJustOpened = true;

            }
            else
            {
                waveInProgress = false;
            }
            // waveInProgress = false;
            waveCooldown = timeBetweenWaves;
            textIndex = 0;
            waveTextAppearTimer = 0f;
            WaveText.text = "";
            waveText = $"Wave {waveCounter + 1}";
            textPhase = true;

            // currentWave.enemiesToSpawn.Clear();
        }
    }

    private void OnGUI()
    {
        if (!waveInProgress)
        {
            WaveOverlay.gameObject.SetActive(true);
            WaveText.gameObject.SetActive(true);
            AnimateText();
        }
        else
        {
            WaveOverlay.gameObject.SetActive(false);
            WaveText.gameObject.SetActive(false);
        }
    }

    private void OnSkillTree()
    {
        if ((waveCounter + 1) % 2 == 0 && isSkillTreeOpen && skillTreeLength < 10)
        {
            SkillTreeOverlay.SetActive(true);
            SkillTreePanel.SetActive(true);
            if (waveCounter + 1 < 20)
            {
                Button[] highTierButtons = GameObject.Find("Tier 2").GetComponentsInChildren<Button>();
                foreach (Button button in highTierButtons)
                {
                    button.interactable = false;
                }
                highTierButtons = GameObject.Find("Tier 3").GetComponentsInChildren<Button>();
                foreach (Button button in highTierButtons)
                {
                    button.interactable = false;
                }
            }
            if (skillTreeJustOpened)
            {
                skillTreeJustOpened = false;
                skillTreeLength = skillTreeManager.skillTree.activeSkills.Count;
            }
            else if (skillTreeLength != skillTreeManager.skillTree.activeSkills.Count)
            {
                skillTreeLength = skillTreeManager.skillTree.activeSkills.Count;
                SkillTreeOverlay.SetActive(false);
                SkillTreePanel.SetActive(false);
                isSkillTreeOpen = false;
                waveInProgress = false;
            }

        }
    }

    private void AnimateText()
    {
        float totalAnimationProgress = 1 - (waveCooldown / timeBetweenWaves);

        if (totalAnimationProgress < 0.5f)
        {
            int charIndex = Mathf.FloorToInt(totalAnimationProgress * 2 * waveText.Length);
            if (charIndex < waveText.Length && waveText[charIndex] == ' ')
            {
                charIndex++;
            }
            WaveText.text = waveText.Substring(0, charIndex + 1);
        }
        else
        {
            int charIndex = Mathf.FloorToInt((1 - totalAnimationProgress) * 2 * waveText.Length);
            if (charIndex >= 0 && waveText[charIndex] == ' ')
            {
                charIndex--;
            }
            WaveText.text = waveText.Substring(0, charIndex + 1);
        }
    }
}
