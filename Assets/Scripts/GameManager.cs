using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string menuSceneName = "MainMenu";
    public float restartDelayOnEnd = 2f;
    public Text scoreText;
    public GameObject victoryPopupGO;

    int score;
    bool hasWon;
    bool isDead;
    int enemyKills;
    public int minKillsForRanking = 5; // valor default (será lido de PlayerPrefs)
    public int CurrentScore => score;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        hasWon = false;
        isDead = false;
        enemyKills = 0;

        // Lê a dificuldade escolhida no menu
        minKillsForRanking = PlayerPrefs.GetInt(MenuManager.PP_MinKillsKey, minKillsForRanking);
        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"SCORE {score:D5}";
    }

    public void RegisterEnemyDestroyed()
    {
        enemyKills++;
    }

    public void OnPlayerHit()
    {
        if (hasWon) return;
        isDead = true;
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    public void OnPlayerWin()
    {
        if (isDead || hasWon) return;
        hasWon = true;
        if (enemyKills >= minKillsForRanking)
        {
            string nameToUse = string.IsNullOrWhiteSpace(LeaderboardManager.CurrentPlayerName)
                ? "Jogador"
                : LeaderboardManager.CurrentPlayerName;
            LeaderboardManager.SubmitScore(nameToUse, score);
        }
        if (victoryPopupGO != null)
        {
            victoryPopupGO.SetActive(true);
            return;
        }
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    System.Collections.IEnumerator ReturnToMenuAfterDelay()
    {
        float end = Time.realtimeSinceStartup + restartDelayOnEnd;
        while (Time.realtimeSinceStartup < end) yield return null;
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
