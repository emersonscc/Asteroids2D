using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryPopupAuto : MonoBehaviour
{
    [Header("Exibição")]
    public Text uiText;                 // UI Text (Legacy) do Title
    public string baseMessage = "VOCÊ VENCEU!";

    [Header("Fluxo")]
    public float holdSeconds = 2f;      // quanto tempo o popup fica na tela
    public bool pauseOnShow = true;
    public string menuSceneName = "MainMenu"; // <- voltar ao menu

    void OnEnable()
    {
        if (!uiText) uiText = GetComponentInChildren<Text>(true);

        var gm = FindFirstObjectByType<GameManager>();
        string msg = baseMessage;
        if (gm != null) msg = $"{baseMessage}\nSCORE {gm.CurrentScore}";
        if (uiText) uiText.text = msg;

        if (pauseOnShow && Time.timeScale != 0f)
            Time.timeScale = 0f;

        StartCoroutine(GoToMenuAfter());
    }

    IEnumerator GoToMenuAfter()
    {
        yield return new WaitForSecondsRealtime(holdSeconds);
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
    }
}
