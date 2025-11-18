using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerCollisionDamage : MonoBehaviour
{
    public PlayerHealth health;

    void Awake()
    {
        if (!health) health = GetComponent<PlayerHealth>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!health) return;

        var other = col.collider;

        // Asteroide: Tag "Asteroid" OU componente Asteroid
        if (other.CompareTag("Asteroid") || other.GetComponent<Asteroid>() != null)
        {
            health.TakeAsteroidCollision();
            return;
        }

        // Inimigo: Tag "Enemy" OU componente Enemy
        if (other.CompareTag("Enemy") || other.GetComponent<Enemy>() != null)
        {
            health.TakeEnemyCollision();
            return;
        }
    }

    // Se alguns desses objetos usarem "Is Trigger", mantenha também:
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!health) return;

        if (other.CompareTag("Asteroid") || other.GetComponent<Asteroid>() != null)
        {
            health.TakeAsteroidCollision();
            return;
        }

        if (other.CompareTag("Enemy") || other.GetComponent<Enemy>() != null)
        {
            health.TakeEnemyCollision();
            return;
        }
    }
}
