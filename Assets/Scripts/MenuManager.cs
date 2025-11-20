using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Cena do jogo")]
    public string gameSceneName = "SampleScene";

    [Header("Entrada de nome")]
    public InputField playerNameInput;   // arraste o PlayerNameInput (Legacy)
    public Button startButton;           // arraste o StartButton

    [Header("Leaderboard (sempre visível)")]
    public Text leaderboardText;         // arraste o Text "LeaderboardText" no Canvas

    void Start()
    {
        // Recarrega último nome usado
        if (playerNameInput)
        {
            playerNameInput.text = LeaderboardManager.CurrentPlayerName;
            playerNameInput.onValueChanged.AddListener(_ => UpdateStartButton());
        }
        UpdateStartButton();

        // Mostra leaderboard imediatamente…
        RefreshLeaderboard();

        // …e de novo um pouquinho depois (garante persistência ao voltar do jogo)
        Invoke(nameof(RefreshLeaderboard), 0.2f);
    }

    void UpdateStartButton()
    {
        if (startButton && playerNameInput)
            startButton.interactable = !string.IsNullOrWhiteSpace(playerNameInput.text);
    }

    public void StartGame()
    {
        if (playerNameInput)
            LeaderboardManager.CurrentPlayerName = playerNameInput.text.Trim();

        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    void RefreshLeaderboard()
    {
        if (leaderboardText)
            leaderboardText.text = LeaderboardManager.FormatLeaderboardText();
    }

    // (Opcional) Sair em build
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
