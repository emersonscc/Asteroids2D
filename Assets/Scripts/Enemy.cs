using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;              // Spawner define; fallback por Tag "Player" no Start

    [Header("Movimento")]
    public float moveSpeed = 3f;
    public float steerStrength = 4f;
    public bool screenWrap = true;

    [Header("Tiro")]
    public Transform firePoint;           // Filho "FirePoint" preferencial; fallback = transform
    public GameObject bulletPrefab;
    public float bulletSpeed = 12f;
    public float fireCooldown = 0.6f;
    public float aimLead = 0.25f;

    [Header("Vida & Pontos")]
    public int maxHealth = 3;             // 3 tiros para destruir
    public int scoreOnKill = 100;

    [Header("Efeito")]
    public GameObject explosionPrefab;    // <- arraste o prefab Explosion aqui

    Rigidbody2D rb;
    int health;
    float fireTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;

        // Garante o FirePoint filho local, se existir
        var childFP = transform.Find("FirePoint");
        if (childFP != null)
            firePoint = childFP;
        else if (firePoint == null || !firePoint.IsChildOf(transform))
            Debug.LogWarning($"[Enemy] FirePoint ausente ou não-filho em {name}. Usando transform como fallback.");
    }

    void Start()
    {
        if (!target)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) target = go.transform;
            else Debug.LogWarning("[Enemy] Player com Tag 'Player' não encontrado.");
        }
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (target)
        {
            AimAndMove();
            if (fireTimer <= 0f) TryShoot();
        }

        if (screenWrap)
            ScreenWrap();
    }

    void AimAndMove()
    {
        Vector2 pos = transform.position;
        Vector2 tgtPos = target.position;
        Vector2 tgtVel = Vector2.zero;

        var trb = target.GetComponent<Rigidbody2D>();
        if (trb) tgtVel = trb.linearVelocity; // use .velocity se seu projeto estiver assim

        Vector2 predicted = tgtPos + tgtVel * aimLead;
        Vector2 dir = (predicted - pos).normalized;

        float ang = Vector2.SignedAngle(transform.up, dir);
        transform.Rotate(0, 0, ang * steerStrength * Time.deltaTime);

        rb.linearVelocity = transform.up * moveSpeed;
    }

    void TryShoot()
    {
        if (!bulletPrefab)
        {
            Debug.LogWarning($"[Enemy] bulletPrefab não definido em {name}");
            fireTimer = 0.2f;
            return;
        }

        var fp = (firePoint && firePoint.IsChildOf(transform)) ? firePoint : transform;

        fireTimer = Mathf.Max(0.05f, fireCooldown);

        Vector2 pos = (Vector2)fp.position + (Vector2)fp.up * 0.1f;
        Quaternion rot = fp.rotation;

        var b = Instantiate(bulletPrefab, pos, rot);
        var brb = b.GetComponent<Rigidbody2D>();
        var own = rb ? rb.linearVelocity : Vector2.zero;

        if (brb)
            brb.linearVelocity = (Vector2)fp.up * bulletSpeed + own * 0.2f;
    }

    public void Hit(int damage = 1)
    {
        health -= Mathf.Max(1, damage);
        if (health <= 0) Die();
    }

    void Die()
    {
        // Explosão
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Pontuação
        var gm = FindFirstObjectByType<GameManager>();
        if (gm) gm.AddScore(scoreOnKill);

        Destroy(gameObject);
    }

    void ScreenWrap()
    {
        var cam = Camera.main;
        var v = cam.WorldToViewportPoint(transform.position);
        if (v.x > 1f) v.x = 0f; else if (v.x < 0f) v.x = 1f;
        if (v.y > 1f) v.y = 0f; else if (v.y < 0f) v.y = 1f;
        var w = cam.ViewportToWorldPoint(v);
        transform.position = new Vector3(w.x, w.y, 0f);
    }
}
