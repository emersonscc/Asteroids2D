using UnityEngine;

public class BulletHit : MonoBehaviour
{
    [Header("Sweep (anti-túnel)")]
    public LayerMask asteroidMask = ~0;
    public float sweepRadius = 0.12f;

    Rigidbody2D rb;
    Vector2 lastPos;
    bool hasHit = false;                // <--- trava

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPos = transform.position;
    }

    void Update()
    {
        if (!hasHit) SweepCheck();      // só varre se ainda não acertou
        lastPos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)    { TryHit(other); }
    void OnCollisionEnter2D(Collision2D col)   { TryHit(col.collider); }

    void SweepCheck()
    {
        Vector2 cur = transform.position;
        Vector2 delta = cur - lastPos;
        float dist = delta.magnitude;
        if (dist <= 0f) return;

        var hit = Physics2D.CircleCast(lastPos, sweepRadius, delta.normalized, dist, asteroidMask);
        if (hit.collider) TryHit(hit.collider);
    }

    void TryHit(Collider2D other)
{
    if (other == null) return;

    // 1) Asteroide
    if (other.TryGetComponent<Asteroid>(out var a))
    {
        a.Hit();
        Destroy(gameObject);
        return;
    }

    // 2) Inimigo
    if (other.TryGetComponent<Enemy>(out var e))
    {
        e.Hit(1);
        Destroy(gameObject);
        return;
    }
}
    
}
