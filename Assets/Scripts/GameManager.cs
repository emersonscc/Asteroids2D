using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs / Spawns")]
    public GameObject asteroidLargePrefab;   // opcional (usado pelo SpawnWave inicial)
    public GameObject playerPrefab;
    public Transform playerSpawn;

    [Header("UI")]
    public Text scoreText;
    public GameObject winPopup;              // opcional: arraste um painel de vitória
    public GameObject gameOverPopup;         // opcional: arraste um painel de game over

    [Header("Jogo")]
    public int lives = 3;

    // Estado interno
    public bool gameEnded;
    int score;

    void Start()
    {
        // Garante que o tempo esteja rodando ao iniciar a cena
        Time.timeScale = 1f;
        gameEnded = false;

        SpawnPlayer();

        // Se quiser começar com alguns asteroides (opcional)
        if (asteroidLargePrefab != null)
            SpawnWave(4);

        UpdateUI();
    }

    // ----- Pontuação / UI -----

    public void AddScore(int value)
    {
        if (gameEnded) return;
        score += value;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText)
            scoreText.text = $"SCORE {score}   LIVES {lives}";
    }

    // ----- Dano / Vidas do Jogador -----

    // Chamado quando o Player "morre" (colisão, tiros inimigos suficientes, etc.)
    public void OnPlayerHit()
    {
        if (gameEnded) return;

        lives--;
        UpdateUI();

        if (lives > 0)
        {
            // Respawn após 1 segundo
            Invoke(nameof(SpawnPlayer), 1.0f);
        }
        else
        {
            // Fim de jogo
            gameEnded = true;
            Debug.Log("Game Over");

            // Desliga spawners/ameaças em andamento
            StopAllSpawnersAndThreats();

            // Mostra UI de game over (opcional)
            if (gameOverPopup) gameOverPopup.SetActive(true);

            // Pausa o jogo
            Time.timeScale = 0f;
        }
    }

    void SpawnPlayer()
    {
        if (gameEnded) return;
        if (!playerPrefab || !playerSpawn)
        {
            Debug.LogWarning("[GameManager] playerPrefab ou playerSpawn não setados.");
            return;
        }

        var player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);

        // Se reaproveitar o mesmo prefab/objeto, zera os hits no respawn
        var ph = player.GetComponent<PlayerHealth>();
        if (ph)
            ph.ResetHealth();
    }

    // ----- Vitória (zona de chegada) -----

    // Chamado pela GoalZone quando o Player entra no trigger do meio (chegou ao planeta)
    public void OnPlayerWin()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("Você alcançou o planeta! Vitória!");

        // Para spawners e "ameaças" ativas
        StopAllSpawnersAndThreats();

        // Mostra UI de vitória (se houver)
        if (winPopup) winPopup.SetActive(true);

        // Pausa o jogo
        Time.timeScale = 0f;
    }

    // ----- Utilidades internas -----

    void StopAllSpawnersAndThreats()
    {
        // Desabilita spawners de inimigos
        var enemySpawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (var sp in enemySpawners)
            sp.enabled = false;

        // Se houver spawner de asteroides no seu projeto, desabilite aqui:
        var asteroidSpawners = FindObjectsOfType<MonoBehaviour>(); // genérico
        foreach (var mb in asteroidSpawners)
        {
            // Se você tiver um script AsteroidSpawner, troque o "mb is" abaixo:
            // if (mb is AsteroidSpawner) mb.enabled = false;
            // Mantive genérico para não quebrar se o script não existir.
        }

        // Opcional: congelar o comportamento dos inimigos existentes
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var e in enemies)
            e.enabled = false;
    }

    // ----- Onda inicial de asteroides (opcional) -----

    void SpawnWave(int count)
    {
        var cam = Camera.main;
        if (!cam || asteroidLargePrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 edge = Random.value > 0.5f
                ? new Vector2(Random.value, Random.value < 0.5f ? -0.05f : 1.05f)
                : new Vector2(Random.value < 0.5f ? -0.05f : 1.05f, Random.value);

            Vector3 world = cam.ViewportToWorldPoint(new Vector3(edge.x, edge.y, 0f));
            world.z = 0f;

            var a = Instantiate(asteroidLargePrefab, world, Quaternion.identity).GetComponent<Asteroid>();
            if (a != null)
            {
                a.size = Asteroid.Size.Large;
                a.Launch(Random.insideUnitCircle.normalized);
            }
        }
    }
}
