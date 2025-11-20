using UnityEngine;
using UnityEngine.UI;

public class HUDScore : MonoBehaviour
{
    public Text uiText;
    GameManager gm;

    void Awake()
    {
        if (!uiText) uiText = GetComponent<Text>();
        gm = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (uiText && gm)
            uiText.text = $"SCORE {gm.CurrentScore}";
    }
}
