using UnityEngine;
// Este script cuida apenas do TIRO. Movimento fica no seu script atual.

public class PlayerShooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform firePoint;          // arraste o filho "FirePoint"
    public GameObject bulletPrefab;      // arraste o prefab "Bullet"

    [Header("Tiro")]
    public float bulletSpeed = 18f;      // velocidade da bala
    public float fireCooldown = 0.15f;   // intervalo mínimo entre tiros
    public float inheritSpeedFactor = 0.3f; // quanto da velocidade da nave a bala herda

    Rigidbody2D rb;      // RB do player (para herdar velocidade)
    float fireTimer;

    void Awake()
    {
        // acha RB no mesmo GO ou no pai
        rb = GetComponent<Rigidbody2D>();
        if (!rb) rb = GetComponentInParent<Rigidbody2D>();

        // fallbacks úteis (evita esquecer no inspector)
        if (firePoint == null)
        {
            var fp = transform.Find("FirePoint");
            if (fp) firePoint = fp;
        }
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        // Espaço pelo input antigo (funciona se Active Input Handling = Both/Old)
        bool firePressed = Input.GetKey(KeyCode.Space);

#if ENABLE_INPUT_SYSTEM
        // Se o projeto estiver só com o Input System (New), descomente esta linha:
        // firePressed = UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed || firePressed;
#endif

        if (firePressed && fireTimer <= 0f)
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("PlayerShooter: bulletPrefab/FirePoint não atribuídos.");
            return;
        }

        fireTimer = fireCooldown;

        // rotação da bala = mesma direção do nariz da nave
        Quaternion rot = firePoint.rotation;        // ou transform.rotation se preferir
        Vector2 forward = (Vector2)firePoint.up;    // direção do tiro

        var go = Instantiate(bulletPrefab, firePoint.position, rot);
        var brb = go.GetComponent<Rigidbody2D>();
        if (brb != null)
        {
            // OBS: Se o seu Rigidbody usa 'velocity' em vez de 'linearVelocity', troque abaixo:
            brb.linearVelocity = forward * bulletSpeed + (rb ? rb.linearVelocity * inheritSpeedFactor : Vector2.zero);
        }
    }

    // Opcional: visualize o FirePoint na cena
    void OnDrawGizmosSelected()
    {
        if (firePoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.up * 0.6f);
        }
    }
}
