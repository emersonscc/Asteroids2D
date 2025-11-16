using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int hitsToDestroy = 5;         // nº de tiros inimigos para destruir
    public bool deactivateOnDeath = true; // compatível com respawn do GameManager

    [Header("Efeito")]
    public GameObject explosionPrefab;    // <- arraste o Explosion aqui

    int hitsTaken = 0;

    // Chamado pela EnemyBullet ao acertar o player
    public void RegisterHit(int damage = 1)
    {
        hitsTaken += Mathf.Max(1, damage);

        if (hitsTaken >= hitsToDestroy)
        {
            // Explosão do player
            if (explosionPrefab)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Notifica o GameManager (vidas/respawn)
            var gm = FindFirstObjectByType<GameManager>();
            if (gm) gm.OnPlayerHit();

            // Some com o player atual (ou desativa para respawn)
            if (deactivateOnDeath)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);

            // Se for reusar o mesmo objeto depois via reativação
            hitsTaken = 0;
        }
    }

    // Útil se reaproveitar o mesmo GO no respawn (reativação)
    public void ResetHealth()
    {
        hitsTaken = 0;
    }
}
