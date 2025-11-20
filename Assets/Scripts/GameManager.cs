using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Cenas & Fluxo")]
    [Tooltip("Nome da cena do Menu principal (índice 0 no Build Profiles).")]
    public string menuSceneName = "MainMenu";
    [Tooltip("Atraso (seg, tempo real) para voltar ao menu quando não houver popup.")]
    public float restartDelayOnEnd = 2f;

    [Header("Player (opcional)")]
    public Transform playerSpawn;
    public GameObject playerPrefab;

    [Header("UI (HUD)")]
    public Text scoreText;                      // Text (Legacy) do HUD

    [Header("Popup de Vitória (opcional)")]
    [Tooltip("Se definido, será ativado na vitória (ex.: Canvas com VictoryPopupAuto).")]
    public GameObject victoryPopupGO;

    // ----- Estado interno -----
    int score;
    bool scoreSubmitted;

    /// <summary>Pontuação atual (para outros scripts/HUDs).</summary>
    public int CurrentScore => score;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        scoreSubmitted = false;
        UpdateUI();

        // Spawn opcional se não houver Player na cena
        if (playerSpawn && playerPrefab && GameObject.FindGameObjectWithTag("Player") == null)
            SpawnPlayer();
    }

    // =================== Player ===================
    public void SpawnPlayer()
    {
        if (playerPrefab && playerSpawn)
            Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
    }

    // =================== Score ====================
    public void AddScore(int value)
    {
        if (value <= 0) return;
        score += value;
        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0;
        scoreSubmitted = false;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText)
            scoreText.text = $"SCORE {score:D5}";
    }

    // ============ Fim de jogo: Morte ==============
    /// <summary>Chame quando o jogador for destruído (tiros/colisões).</summary>
    public void OnPlayerHit()
    {
        SubmitScoreOnce();
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    // ============ Fim de jogo: Vitória =============
    /// <summary>Chame quando o jogador atingir o objetivo (ex.: Goal/Planeta).</summary>
    public void OnPlayerWin()
    {
        SubmitScoreOnce();

        if (victoryPopupGO != null)
        {
            // Seu VictoryPopupAuto cuida de pausar/mostrar texto/voltar ao menu.
            victoryPopupGO.SetActive(true);
            return;
        }

        // Fallback simples: volta ao menu após um atraso
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    // ============ Leaderboard ======================
    void SubmitScoreOnce()
    {
        if (scoreSubmitted) return;

        // Nome atual foi salvo pelo MenuManager antes de iniciar o jogo
        LeaderboardManager.SubmitScore(
            LeaderboardManager.CurrentPlayerName,
            score
        );
        scoreSubmitted = true;
    }

    // ============ Navegação ========================
    System.Collections.IEnumerator ReturnToMenuAfterDelay()
    {
        // Usa tempo real para independência de Time.timeScale
        float end = Time.realtimeSinceStartup + restartDelayOnEnd;
        while (Time.realtimeSinceStartup < end)
            yield return null;

        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
    }
}
