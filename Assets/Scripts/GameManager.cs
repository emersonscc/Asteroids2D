using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs / Referências")]
    public GameObject asteroidLargePrefab;
    public GameObject playerPrefab;
    public Transform playerSpawn;

    [Header("Spawns")]
    public float spawnInterval = 2f;
    public float minSpawnDistanceFromPlayer = 4f;

    Transform playerTransform;

    void Start()
    {
        SpawnPlayer();
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            SpawnOneAsteroid();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPlayer()
    {
        var p = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
        playerTransform = p.transform;
        UpdateAsteroidTargets();
    }

    void SpawnOneAsteroid()
    {
        if (!asteroidLargePrefab) { Debug.LogError("GameManager: asteroidLargePrefab não setado."); return; }
        Camera cam = Camera.main;

        // escolhe uma borda da tela
        Vector2 edge = Random.value > 0.5f
            ? new Vector2(Random.value, Random.value < 0.5f ? -0.05f : 1.05f)
            : new Vector2(Random.value < 0.5f ? -0.05f : 1.05f, Random.value);

        Vector3 world = cam.ViewportToWorldPoint(new Vector3(edge.x, edge.y, 0));
        world.z = 0;

        // evita spawn colado na nave
        if (playerTransform != null)
        {
            int safety = 0;
            while (Vector2.Distance(world, playerTransform.position) < minSpawnDistanceFromPlayer && safety++ < 8)
            {
                edge = Random.value > 0.5f
                    ? new Vector2(Random.value, Random.value < 0.5f ? -0.05f : 1.05f)
                    : new Vector2(Random.value < 0.5f ? -0.05f : 1.05f, Random.value);
                world = cam.ViewportToWorldPoint(new Vector3(edge.x, edge.y, 0));
                world.z = 0;
            }
        }

        var go = Instantiate(asteroidLargePrefab, world, Quaternion.identity);
        var a = go.GetComponent<Asteroid>();
        if (a != null)
        {
            a.size = Asteroid.Size.Large;
            a.target = playerTransform;
            Vector2 dir = (playerTransform != null)
                ? ((Vector2)(playerTransform.position - world)).normalized
                : Random.insideUnitCircle.normalized;
            dir = (dir + Random.insideUnitCircle * 0.2f).normalized;
            a.Launch(dir);
        }
    }

    void UpdateAsteroidTargets()
    {
#if UNITY_2022_2_OR_NEWER
        var all = Object.FindObjectsByType<Asteroid>(FindObjectsSortMode.None);
#else
        var all = Object.FindObjectsOfType<Asteroid>();
#endif
        foreach (var ast in all) ast.target = playerTransform;
    }
}
