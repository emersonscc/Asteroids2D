using UnityEngine;
using UnityEngine.UI;
// Opcional: se você usa TextMeshPro, deixe o using abaixo e preencha o campo scoreTMP no Inspector
// using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerPrefab;        // Prefab do jogador
    public Transform playerSpawn;          // Posição de respawn

    [Header("Jogo")]
    public int lives = 3;                  // Vidas iniciais
    public float respawnDelay = 1.0f;      // Tempo para respawn do player

    [Header("UI (opcional)")]
    public Text scoreText;                 // UI Text padrão
    // public TMP_Text scoreTMP;           // Se usar TextMeshPro, descomente acima e preencha no Inspector

    int score = 0;
    GameObject currentPlayer;

    void Start()
    {
        UpdateUI();
        SpawnPlayer();
    }

    /// <summary>
    /// Adiciona pontos ao placar (usado por inimigos/asteroides destruídos).
    /// </summary>
    public void AddScore(int value)
    {
        score += Mathf.Max(0, value);
        UpdateUI();
    }

    /// <summary>
    /// Chamado quando o jogador é atingido (por asteroide, inimigo, tiro inimigo, etc).
    /// </summary>
    public void OnPlayerHit()
    {
        // Se já está “morto” e aguardando respawn, evita múltiplas contagens.
        if (currentPlayer == null || !currentPlayer.activeInHierarchy)
        {
            // já está sem player ativo; ainda assim processa vida/respawn abaixo
        }
        else
        {
            // desativa o player atual
            currentPlayer.SetActive(false);
        }

        lives = Mathf.Max(0, lives - 1);
        UpdateUI();

        if (lives > 0)
        {
            // respawn após um pequeno atraso
            Invoke(nameof(SpawnPlayer), respawnDelay);
        }
        else
        {
            // fim de jogo simples (você pode expandir com tela de Game Over, etc.)
            Debug.Log("Game Over");
            // opcional: desabilitar spawners, mostrar UI, etc.
        }
    }

    /// <summary>
    /// Instancia/respawna o jogador no ponto definido.
    /// </summary>
    void SpawnPlayer()
    {
        if (playerPrefab == null || playerSpawn == null)
        {
            Debug.LogWarning("GameManager: defina Player Prefab e Player Spawn no Inspector.");
            return;
        }

        // Se existe um player antigo na cena (desativado), destrua para evitar duplicata oculta
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation);
        // Garante a tag (se você usa Tag para detecção)
        if (!currentPlayer.CompareTag("Player"))
            currentPlayer.tag = "Player";
    }

    /// <summary>
    /// Atualiza texto de score/vidas se os campos estiverem atribuídos.
    /// </summary>
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"SCORE {score}   LIVES {lives}";

        // Se usar TextMeshPro, descomente os campos/using lá em cima e esta linha:
        // if (scoreTMP != null) scoreTMP.text = $"SCORE {score}   LIVES {lives}";
    }

    // Helpers públicos (opcionais) para outros scripts consultarem o player atual
    public Transform GetPlayerTransform()
    {
        return currentPlayer != null ? currentPlayer.transform : null;
    }

    public int GetScore() => score;
    public int GetLives() => lives;
}
