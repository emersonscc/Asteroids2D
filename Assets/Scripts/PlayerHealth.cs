using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Dano por TIROS inimigos")]
    [Tooltip("Quantos tiros inimigos o jogador pode levar antes de ser destruído.")]
    public int hitsToDestroy = 5;

    [Header("Dano por COLISÕES")]
    [Tooltip("Colisões com asteroides para destruir o jogador.")]
    public int collisionsWithAsteroidsToDestroy = 3;

    [Tooltip("Colisões com inimigos para destruir o jogador.")]
    public int collisionsWithEnemiesToDestroy = 2;

    [Header("Feedback")]
    public GameObject explosionPrefab;

    int bulletHits;                 // tiros recebidos
    int asteroidCollisionHits;      // colisões com asteroides
    int enemyCollisionHits;         // colisões com inimigos
    bool dead;

    // Chamado no respawn pelo GameManager (se precisar)
    public void ResetHealth()
    {
        bulletHits = 0;
        asteroidCollisionHits = 0;
        enemyCollisionHits = 0;
        dead = false;
        gameObject.SetActive(true);
    }

    // ===== TIRO inimigo =====
    public void TakeBulletHit()
    {
        if (dead) return;
        bulletHits++;
        if (bulletHits >= hitsToDestroy) Die();
    }

    // ===== COLISÃO com ASTEROIDE =====
    public void TakeAsteroidCollision()
    {
        if (dead) return;
        asteroidCollisionHits++;
        if (asteroidCollisionHits >= collisionsWithAsteroidsToDestroy) Die();
    }

    // ===== COLISÃO com INIMIGO =====
    public void TakeEnemyCollision()
    {
        if (dead) return;
        enemyCollisionHits++;
        if (enemyCollisionHits >= collisionsWithEnemiesToDestroy) Die();
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.OnPlayerHit();

        gameObject.SetActive(false);
    }
}
