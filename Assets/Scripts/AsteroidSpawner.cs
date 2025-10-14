using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidLargePrefab;
    public float spawnInterval = 2f;

    void Start() => StartCoroutine(SpawnLoop());

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            SpawnAtEdge();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnAtEdge()
    {
        if (!asteroidLargePrefab) return;

        var cam = Camera.main;
        // escolhe uma borda aleatória da viewport
        Vector2 edge = Random.value > 0.5f
            ? new Vector2(Random.value, Random.value < 0.5f ? -0.05f : 1.05f)
            : new Vector2(Random.value < 0.5f ? -0.05f : 1.05f, Random.value);

        Vector3 world = cam.ViewportToWorldPoint(new Vector3(edge.x, edge.y, 0));
        world.z = 0;

        var go = Instantiate(asteroidLargePrefab, world, Quaternion.identity);
        var a = go.GetComponent<Asteroid>();
        if (a != null)
        {
            // direção aleatória para dentro
            Vector2 dir = (Vector2)(cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)) - world);
            dir = (dir.normalized + Random.insideUnitCircle * 0.3f).normalized;
            a.Launch(dir);
        }
    }
}
