using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float thrust = 7f;
    public float rotationSpeed = 180f;
    public float maxSpeed = 10f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 18f;
    public float fireCooldown = 0.15f;

    Rigidbody2D rb;
    float fireTimer;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        // Rotaciona com A/D ou ←/→
        float rot = -Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, 0, rot);

        // Atira com Espaço ou clique
        fireTimer -= Time.deltaTime;
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && fireTimer <= 0f)
        {
            fireTimer = fireCooldown;
            /*var b = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            var brb = b.GetComponent<Rigidbody2D>();
            Vector2 forward = (Vector2)transform.up;             // converte para Vector2
            brb.linearVelocity = forward * bulletSpeed + rb.linearVelocity * 0.3f;*/
            // 1) Posição do mouse em coordenadas de mundo
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f; // garantimos o plano 2D

            // 2) Direção da bala: do firePoint até o mouse
            Vector2 dir = ((Vector2)(mouseWorld - firePoint.position)).normalized;

            // 3) Opcional: rotacionar a bala para "apontar" visualmente
            Quaternion bulletRot = Quaternion.FromToRotation(Vector3.up, (Vector3)dir);

            // 4) Instancia e dá velocidade nessa direção
            GameObject b = Instantiate(bulletPrefab, firePoint.position, bulletRot);
            Rigidbody2D brb = b.GetComponent<Rigidbody2D>();
            brb.linearVelocity = dir * bulletSpeed + rb.linearVelocity * 0.3f;
        }

        ScreenWrap();
    }

    void FixedUpdate()
    {
        // Acelera com W ou ↑
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            rb.AddForce(transform.up * thrust);

        // Limite de velocidade
        if (rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void ScreenWrap()
    {
        var cam = Camera.main;
        var view = cam.WorldToViewportPoint(transform.position);

        if (view.x > 1) view.x = 0; else if (view.x < 0) view.x = 1;
        if (view.y > 1) view.y = 0; else if (view.y < 0) view.y = 1;

        transform.position = cam.ViewportToWorldPoint(view);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Asteroid"))
        {
            Object.FindFirstObjectByType<GameManager>().OnPlayerHit();
            gameObject.SetActive(false);
        }
    }
}
