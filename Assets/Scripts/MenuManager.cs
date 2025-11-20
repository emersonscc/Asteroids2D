using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Tooltip("Nome da cena do jogo")]
    public string gameSceneName = "Game";

    public void StartGame()
    {
        Time.timeScale = 1f; // garante tempo normal
        if (!string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(gameSceneName);
    }

    // Se quiser sair do jogo em builds:
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
