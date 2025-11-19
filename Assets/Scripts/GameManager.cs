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

    [Header("Restart ao morrer")]
    public bool restartOnDeath = true;
    public float restartDelayOnDeath = 2f;

    int score;
    bool gameEnded;

    // Exposição do score para o popup
    public int CurrentScore => score;

    void Start()
    {
        if (winPopup) winPopup.SetActive(false);
        SpawnPlayer();
        SpawnWave(4);
        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"SCORE {score}   LIVES {lives}";
    }

    public void OnPlayerHit()
    {
        if (gameEnded) return;

        lives--;
        UpdateUI();

        if (restartOnDeath)
        {
            if (Time.timeScale != 0f) Time.timeScale = 0f;
            StartCoroutine(RestartAfterDeathDelay());
            return;
        }

        if (lives > 0) Invoke(nameof(SpawnPlayer), 1.0f);
        else Debug.Log("Game Over");
    }

    System.Collections.IEnumerator RestartAfterDeathDelay()
    {
        yield return new WaitForSecondsRealtime(restartDelayOnDeath);
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void OnPlayerWin()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (winPopup) winPopup.SetActive(true);

        // Pausa para o popup de vitória; o VictoryPopupAuto reinicia depois
        if (Time.timeScale != 0f) Time.timeScale = 0f;
    }

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
