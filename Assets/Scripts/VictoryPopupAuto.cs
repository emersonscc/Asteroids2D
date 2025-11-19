using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if TMP_PRESENT || TEXTMESHPRO_PRESENT
using TMPro;
#endif

public class VictoryPopupAuto : MonoBehaviour
{
    [Header("Exibição")]
    public Text uiText;          // Legacy UI Text (opcional)
#if TMP_PRESENT || TEXTMESHPRO_PRESENT
    public TMP_Text tmpText;     // TextMeshPro (opcional)
#endif
    public string baseMessage = "VOCÊ VENCEU!";

    [Header("Fluxo")]
    public float holdSeconds = 2f;
    public bool pauseOnShow = true;

    void OnEnable()
    {
        // Tenta auto-descobrir textos se não estiverem atribuídos
        if (!uiText)
            uiText = GetComponentInChildren<Text>(true);
#if TMP_PRESENT || TEXTMESHPRO_PRESENT
        if (!tmpText)
            tmpText = GetComponentInChildren<TMP_Text>(true);
#endif

        // Monta mensagem com score atual
        var gm = FindFirstObjectByType<GameManager>();
        string msg = baseMessage;
        if (gm) msg = $"{baseMessage}\nSCORE {gm.CurrentScore}";

        if (uiText) uiText.text = msg;
#if TMP_PRESENT || TEXTMESHPRO_PRESENT
        if (tmpText) tmpText.text = msg;
#endif

        if (pauseOnShow && Time.timeScale != 0f)
            Time.timeScale = 0f;

        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSecondsRealtime(holdSeconds);
        Time.timeScale = 1f;
        var cur = SceneManager.GetActiveScene();
        SceneManager.LoadScene(cur.buildIndex);
    }
}
