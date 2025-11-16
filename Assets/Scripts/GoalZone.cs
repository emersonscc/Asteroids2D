using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [Tooltip("Tag do objeto que conta como chegada")]
    public string playerTag = "Player";

    [Tooltip("UI opcional que aparece ao vencer")]
    public GameObject winPopup;

    [Tooltip("Pausar o jogo no momento da vitória")]
    public bool pauseOnWin = true;

    bool triggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        // Aceita por Tag ou pela presença do PlayerController
        if (other.CompareTag(playerTag) || other.GetComponent<PlayerController>())
        {
            triggered = true;

            var gm = FindFirstObjectByType<GameManager>();
            if (gm) gm.OnPlayerWin();

            if (winPopup) winPopup.SetActive(true);
            if (pauseOnWin) Time.timeScale = 0f;

            // evita reprocessar
            enabled = false;
        }
    }
}
