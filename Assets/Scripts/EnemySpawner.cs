using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab do inimigo")]
    public GameObject enemyPrefab;

    [Header("Tempo de spawn")]
    public float firstDelay = 5f;         // atraso inicial antes do primeiro inimigo
    public float interval = 8f;           // intervalo médio entre spawns
    public float intervalRandom = 3f;     // variação (+/-) no intervalo

    [Header("Regras de spawn")]
    public float minDistanceFromPlayer = 4f;   // evita nascer muito perto do player
    public float offscreenMargin = 0.05f;      // margem fora da tela (viewport)

    private Transform player;                  // alvo a ser atribuído aos inimigos

    void Start()
    {
        // Tenta achar o player de forma robusta
        var pc = FindFirstObjectByType<PlayerController>();
        if (pc) player = pc.transform;

        if (!player)
        {
            var pGo = GameObject.FindGameObjectWithTag("Player");
            if (pGo) player = pGo.transform;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        if (firstDelay > 0f)
            yield return new WaitForSeconds(firstDelay);

        while (true)
        {
            SpawnEnemy();

            float wait = interval + Random.Range(-intervalRandom, intervalRandom);
            if (wait < 2f) wait = 2f; // limite mínimo razoável
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnEnemy()
    {
        if (!enemyPrefab) return;

        var cam = Camera.main;
        if (!cam) return;

        // Escolhe uma borda da viewport (com pequena margem fora da tela)
        Vector2 edge = Random.value > 0.5f
            ? new Vector2(Random.value, Random.value < 0.5f ? -offscreenMargin : 1f + offscreenMargin)
            : new Vector2(Random.value < 0.5f ? -offscreenMargin : 1f + offscreenMargin, Random.value);

        Vector3 pos = cam.ViewportToWorldPoint(new Vector3(edge.x, edge.y, 0f));
        pos.z = 0f;

        // Se ficar perto demais do player, empurra para fora
        if (player && Vector2.Distance(pos, player.position) < minDistanceFromPlayer)
        {
            Vector2 dir = (Vector2)(pos - (Vector3)player.position);
            if (dir.sqrMagnitude < 0.01f) dir = Random.insideUnitCircle.normalized;
            pos = player.position + (Vector3)(dir.normalized * minDistanceFromPlayer);
        }

        // Instancia o inimigo
        var go = Instantiate(enemyPrefab, pos, Quaternion.identity);

        // Injeta o alvo explicitamente (mesmo que a tag falhe)
        var enemy = go.GetComponent<Enemy>();
        if (enemy)
        {
            // Garante o player (rebusca se necessário)
            if (!player)
            {
                var pc = FindFirstObjectByType<PlayerController>();
                if (pc) player = pc.transform;

                if (!player)
                {
                    var pGo = GameObject.FindGameObjectWithTag("Player");
                    if (pGo) player = pGo.transform;
                }
            }

            enemy.target = player;

            // (Opcional) já rotaciona o inimigo olhando para o player
            if (player)
            {
                Vector2 toPlayer = (Vector2)(player.position - go.transform.position);
                if (toPlayer.sqrMagnitude > 0.001f)
                {
                    float z = Vector2.SignedAngle(Vector2.up, toPlayer.normalized);
                    go.transform.rotation = Quaternion.Euler(0f, 0f, z);
                }
            }
        }
    }
}

