using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{
    public float offscreenMargin = 0.05f;

    void Update()
    {
        var cam = Camera.main;
        var v = cam.WorldToViewportPoint(transform.position);
        if (v.x < -offscreenMargin || v.x > 1f + offscreenMargin ||
            v.y < -offscreenMargin || v.y > 1f + offscreenMargin)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Acertou o player?
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>())
        {
            // Prioriza usar PlayerHealth (contagem de hits)
            var ph = other.GetComponent<PlayerHealth>();
            if (ph)
            {
                ph.RegisterHit(1); // 1 hit por bala
            }
            else
            {
                // Fallback: comportamento antigo (morte imediata) se PlayerHealth não existir
                var gm = FindFirstObjectByType<GameManager>();
                if (gm) gm.OnPlayerHit();
            }

            Destroy(gameObject);
        }
    }
}
