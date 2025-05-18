using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void RestartGame()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
    }
}
