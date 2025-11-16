using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryPopupAuto : MonoBehaviour
{
    [Tooltip("Segundos (tempo real) antes de reiniciar a cena")]
    public float holdSeconds = 2f;

    [Tooltip("Pausar o jogo quando o popup abrir")]
    public bool pauseOnShow = true;

    void OnEnable()
    {
        if (pauseOnShow && Time.timeScale != 0f)
            Time.timeScale = 0f;

        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        // usa tempo real para funcionar mesmo com o jogo pausado
        yield return new WaitForSecondsRealtime(holdSeconds);

        Time.timeScale = 1f;
        var cur = SceneManager.GetActiveScene();
        SceneManager.LoadScene(cur.buildIndex);
    }
}
