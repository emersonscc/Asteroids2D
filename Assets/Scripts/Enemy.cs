using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Vida & Pontos")]
    public int maxHealth = 3;
    public int scoreOnKill = 100;

    [Header("Movimento")]
    public float moveSpeed = 3f;
    public float steerStrength = 4f;
    public bool screenWrap = true;

    [Header("Tiro")]
    public Transform firePoint;           // filho "FirePoint"
    public GameObject bulletPrefab;       // prefab da bala do inimigo
    public float bulletSpeed = 12f;
    public float fireCooldown = 0.6f;
    public float aimLead = 0.25f;

    [Header("Alvo")]
    public Transform target;              // defina para o player

    Rigidbody2D rb;
    int health;
    float fireTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        if (!firePoint)
        {
            var fp = transform.Find("FirePoint");
            if (fp) firePoint = fp;
        }
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        if (target) AimAndMove();
        if (target && fireTimer <= 0f) TryShoot();
        if (screenWrap) ScreenWrap();
    }

    void AimAndMove()
    {
        Vector2 pos = transform.position;
        Vector2 tgtPos = target.position;
        Vector2 tgtVel = Vector2.zero;

        var trb = target.GetComponent<Rigidbody2D>();
        if (trb) tgtVel = trb.linearVelocity; // troque para trb.velocity se usar 'velocity'

        Vector2 predicted = tgtPos + tgtVel * aimLead;
        Vector2 dir = (predicted - pos).normalized;

        float ang = Vector2.SignedAngle(transform.up, dir);
        transform.Rotate(0, 0, ang * steerStrength * Time.deltaTime);

        rb.linearVelocity = transform.up * moveSpeed; // troque para rb.velocity se preferir
    }

    void TryShoot()
{
    if (fireTimer > 0f) return;

    if (!bulletPrefab)
    {
        Debug.LogWarning($"[Enemy] bulletPrefab não definido em {name}");
        fireTimer = 0.2f;
        return;
    }

    // Se esqueceram de ligar no Inspector, usa o transform do Enemy
    var fp = firePoint ? firePoint : transform;

    fireTimer = Mathf.Max(0.05f, fireCooldown);

    // Nasce um pouco à frente para não colidir com o próprio colisor
    Vector2 pos = (Vector2)fp.position + (Vector2)fp.up * 0.1f;
    Quaternion rot = fp.rotation;

    var b = Instantiate(bulletPrefab, pos, rot);
    var brb = b.GetComponent<Rigidbody2D>();
    var own = rb ? rb.linearVelocity : Vector2.zero; // troque para rb.velocity se seu projeto usa 'velocity'

    Vector2 dir = (Vector2)fp.up;
    float speed = Mathf.Max(1f, bulletSpeed);

    if (brb)
        brb.linearVelocity = dir * speed + own * 0.2f;

    // Debug opcional:
    // Debug.Log($"[Enemy] Shoot {name} em {Time.time:F2}");
}

    public void Hit(int damage = 1)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    void Die()
    {
        var gm = FindFirstObjectByType<GameManager>();
        if (gm) gm.AddScore(scoreOnKill);   // << CORRETO: método, não delegate
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

    void Start()
{
    // Se o target não vier do spawner, procura o Player por tag
    if (!target)
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go) target = go.transform;
        else Debug.LogWarning("[Enemy] Player com Tag 'Player' não encontrado.");
    }
}

}
