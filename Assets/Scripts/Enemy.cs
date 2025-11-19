using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;

    [Header("Movimento")]
    public float moveSpeed = 3f;
    public float steerStrength = 4f;
    public bool screenWrap = true;

    [Header("Tiro")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 12f;
    public float fireCooldown = 0.6f;
    public float aimLead = 0.25f;

    [Header("Vida & Pontos")]
    public int maxHealth = 3;
    public int scoreOnKill = 50;        // <- 50 pontos por inimigo destruído

    [Header("Efeito")]
    public GameObject explosionPrefab;

    Rigidbody2D rb;
    int health;
    float fireTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;

        var childFP = transform.Find("FirePoint");
        if (childFP) firePoint = childFP;
    }

    void Start()
    {
        if (!target)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) target = go.transform;
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

        if (screenWrap) ScreenWrap();
    }

    void AimAndMove()
    {
        Vector2 pos = transform.position;
        Vector2 tgtPos = target.position;
        Vector2 tgtVel = Vector2.zero;

        var trb = target.GetComponent<Rigidbody2D>();
        if (trb) tgtVel = trb.linearVelocity;

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
            fireTimer = 0.2f;
            return;
        }

        var fp = firePoint ? firePoint : transform;
        fireTimer = Mathf.Max(0.05f, fireCooldown);

        Vector2 pos = (Vector2)fp.position + (Vector2)fp.up * 0.1f;
        Quaternion rot = fp.rotation;

        var b = Instantiate(bulletPrefab, pos, rot);
        var brb = b.GetComponent<Rigidbody2D>();
        var own = rb ? rb.linearVelocity : Vector2.zero;

        if (brb) brb.linearVelocity = (Vector2)fp.up * bulletSpeed + own * 0.2f;
    }

    public void Hit(int damage = 1)
    {
        health -= Mathf.Max(1, damage);
        if (health <= 0) Die();
    }

    void Die()
    {
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        var gm = FindFirstObjectByType<GameManager>();
        if (gm) gm.AddScore(scoreOnKill);   // <- soma 50 pontos

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
