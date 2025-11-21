/*using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class MenuManager : MonoBehaviour
{
    public string gameSceneName = "Game";
    public InputField playerNameInput;
    public Button easyButton;
    public Button hardButton;
    public Text leaderboardText;
    public Text errorText;
    public bool autoRefreshLeaderboard = true;
    public float refreshDelay = 0.2f;
    public const string PP_MinKillsKey = "minKillsForRanking_menu";

    const int MinLen = 2;
    static readonly Regex Allowed = new Regex(@"^[\p{L}\p{Nd} ]+$", RegexOptions.Compiled);

    void Start()
    {
        playerNameInput.text = LeaderboardManager.CurrentPlayerName;
        playerNameInput.onValueChanged.AddListener(_ => RefreshButtons());
        RefreshButtons();
        RefreshLeaderboard();
        if (autoRefreshLeaderboard) Invoke(nameof(RefreshLeaderboard), refreshDelay);
        easyButton.onClick.AddListener(StartEasy);
        hardButton.onClick.AddListener(StartHard);
    }

    void RefreshButtons()
    {
        bool ok = IsValid(playerNameInput.text);
        easyButton.interactable = ok;
        hardButton.interactable = ok;
        if (errorText)
            errorText.text = ok ? "" : "Informe um nome (mín. 2 caracteres, apenas letras/números).";
    }

    bool IsValid(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return false;
        string t = raw.Trim();
        if (t.Length < MinLen) return false;
        if (!Allowed.IsMatch(t)) return false;
        return true;
    }

    public void StartEasy()  { StartWithDifficulty(5); }
    public void StartHard()  { StartWithDifficulty(10); }

    void StartWithDifficulty(int minKillsRequired)
    {
        if (!IsValid(playerNameInput.text)) { RefreshButtons(); return; }
        LeaderboardManager.CurrentPlayerName = playerNameInput.text.Trim();
        PlayerPrefs.SetInt(PP_MinKillsKey, minKillsRequired);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void RefreshLeaderboard()
    {
        if (leaderboardText)
            leaderboardText.text = LeaderboardManager.FormatLeaderboardText();
    }

    void Update()
{
    // Ao pressionar R, apaga todo o ranking
    if (Input.GetKeyDown(KeyCode.R))
    {
        LeaderboardManager.ClearLeaderboard();
        RefreshLeaderboard(); // atualiza o texto no menu
        Debug.Log("Ranking resetado.");
    }
}
}*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class MenuManager : MonoBehaviour
{
    public string gameSceneName = "Game";
    public InputField playerNameInput;
    public Button easyButton;
    public Button hardButton;
    public Text leaderboardText;
    public Text errorText;
    public bool autoRefreshLeaderboard = true;
    public float refreshDelay = 0.2f;
    public const string PP_MinKillsKey = "minKillsForRanking_menu";

    const int MinLen = 2;
    static readonly Regex Allowed = new Regex(@"^[\p{L}\p{Nd} ]+$", RegexOptions.Compiled);

    void Start()
    {
        playerNameInput.text = LeaderboardManager.CurrentPlayerName;
        playerNameInput.onValueChanged.AddListener(_ => RefreshButtons());
        RefreshButtons();
        RefreshLeaderboard();
        if (autoRefreshLeaderboard) Invoke(nameof(RefreshLeaderboard), refreshDelay);
        easyButton.onClick.AddListener(StartEasy);
        hardButton.onClick.AddListener(StartHard);
    }

    void RefreshButtons()
    {
        bool ok = IsValid(playerNameInput.text);
        easyButton.interactable = ok;
        hardButton.interactable = ok;
        if (errorText)
            errorText.text = ok ? "" : "Informe um nome (mín. 2 caracteres, apenas letras/números).";
    }

    bool IsValid(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return false;
        string t = raw.Trim();
        if (t.Length < MinLen) return false;
        if (!Allowed.IsMatch(t)) return false;
        return true;
    }

    public void StartEasy()  { StartWithDifficulty(5); }
    public void StartHard()  { StartWithDifficulty(10); }

    void StartWithDifficulty(int minKillsRequired)
    {
        if (!IsValid(playerNameInput.text)) { RefreshButtons(); return; }
        LeaderboardManager.CurrentPlayerName = playerNameInput.text.Trim();
        PlayerPrefs.SetInt(PP_MinKillsKey, minKillsRequired);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void RefreshLeaderboard()
    {
        if (leaderboardText)
            leaderboardText.text = LeaderboardManager.FormatLeaderboardText();
    }

    void Update()
    {
        // Reset do ranking (tecla R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            LeaderboardManager.ClearLeaderboard();
            RefreshLeaderboard();
            Debug.Log("Ranking resetado.");
        }

        // Sair do jogo (Esc ou F10)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F10))
        {
            QuitGame();
        }
    }

    // Pode ligar este método a um botão "Sair" no menu, se quiser
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // para no Editor
#else
        Application.Quit(); // fecha o executável
#endif
    }
}
