using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs e Spawns")]
    public GameObject asteroidLargePrefab;
    public Transform playerSpawn;
    public GameObject playerPrefab;

    [Header("UI")]
    public Text scoreText;
    public GameObject winPopup;                // painel de vitória (desativado por padrão)

    [Header("Jogo")]
    public int lives = 3;

    [Header("Cenas")]
    public string menuSceneName = "MainMenu";  // <- nome da cena do menu
    public string gameSceneName = "Game";      // opcional, se precisar

    [Header("Tempos")]
    public float restartDelayOnDeath = 2f;     // delay para voltar ao menu quando morrer

    int score;
    bool gameEnded;

    public int CurrentScore => score;

    void Start()
    {
        if (winPopup) winPopup.SetActive(false);
        SpawnPlayer();
        SpawnWave(4);
        UpdateUI();
    }

    // ======= Pontuação =======
    public void AddScore(int value)
    {
        score += value;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"SCORE {score}   LIVES {lives}";
    }

    // ======= Jogador morreu (tiros/colisões) -> voltar ao menu =======
    public void OnPlayerHit()
    {
        if (gameEnded) return;

        lives--;
        UpdateUI();

        // Sempre voltar ao menu na morte
        if (Time.timeScale != 0f) Time.timeScale = 0f;
        StartCoroutine(ReturnToMenuAfter(restartDelayOnDeath));
    }

    System.Collections.IEnumerator ReturnToMenuAfter(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
    }

    // ======= Vitória (goal no planeta) =======
    public void OnPlayerWin()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (winPopup) winPopup.SetActive(true);  // popup mostra SCORE e segura ~2s
        if (Time.timeScale != 0f) Time.timeScale = 0f; // pausa para o popup
        // O retorno ao menu, no caso da vitória, será feito pelo VictoryPopupAuto.
    }

    // ======= Utilidades =======
    void SpawnPlayer()
    {
        if (playerPrefab && playerSpawn)
            Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
    }

    void SpawnWave(int count)
    {
        if (!asteroidLargePrefab) return;
        var cam = Camera.main;

        for (int i = 0; i < count; i++)
        {
            Vector2 edge = Random.value > 0.5f
                ? new Vector2(Random.value, Random.value < 0.5f ? -0.05f : 1.05f)
                : new Vector2(Random.value < 0.5f ? -0.05f : 1.05f, Random.value);

            Vector3 world = cam.ViewportToWorldPoint(new Vector3(edge.x, edge.y, 0));
            world.z = 0;

            var go = Instantiate(asteroidLargePrefab, world, Quaternion.identity);
            var a = go.GetComponent<Asteroid>();
            if (a != null)
            {
                a.size = Asteroid.Size.Large;
                a.Launch(Random.insideUnitCircle.normalized);
            }
        }
    }
}
