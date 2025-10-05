using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject asteroidLargePrefab;
    public Transform playerSpawn;
    public GameObject playerPrefab;
    public Text scoreText;
    public int lives = 3;
    int score;

    void Start()
    {
        SpawnPlayer();
        SpawnWave(4);
        UpdateUI();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"SCORE {score}   LIVES {lives}";
    }

    public void OnPlayerHit()
    {
        lives--;
        UpdateUI();
        if (lives > 0) Invoke(nameof(SpawnPlayer), 1.0f);
        else Debug.Log("Game Over");
    }

    void SpawnPlayer()
    {
        Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
    }

    void SpawnWave(int count)
    {
        Camera cam = Camera.main;
        for (int i = 0; i < count; i++)
        {
            Vector2 edge = Random.value > 0.5f
                ? new Vector2(Random.value, Random.value < 0.5f ? -0.05f : 1.05f)
                : new Vector2(Random.value < 0.5f ? -0.05f : 1.05f, Random.value);

            Vector3 world = cam.ViewportToWorldPoint(new Vector3(edge.x, edge.y, 0));
            world.z = 0;
            var a = Instantiate(asteroidLargePrefab, world, Quaternion.identity).GetComponent<Asteroid>();
            a.size = Asteroid.Size.Large;
            a.Launch(Random.insideUnitCircle.normalized);
        }
    }
}
