using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{
    [Tooltip("Tempo de vida da bala (segundos)")]
    public float lifeSeconds = 3f;

    void OnEnable()
    {
        if (lifeSeconds > 0f)
            Invoke(nameof(SelfDestruct), lifeSeconds);
    }

    // Caso o collider da bala esteja como "Is Trigger"
    void OnTriggerEnter2D(Collider2D other)
    {
        var ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeBulletHit();
            SelfDestruct();
        }
    }

    // Caso NÃO seja trigger e use física 2D padrão
    void OnCollisionEnter2D(Collision2D col)
    {
        var ph = col.collider.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeBulletHit();
            SelfDestruct();
        }
    }

    void SelfDestruct()
    {
        if (this != null && gameObject != null)
            Destroy(gameObject);
    }
}
