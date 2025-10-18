using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public enum Size { Large, Medium, Small }

    [Header("Tamanho / Divisão")]
    public Size size = Size.Large;
    public GameObject mediumPrefab;   // usado quando Large quebra
    public GameObject smallPrefab;    // usado quando Medium quebra

    [Header("Movimento")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 3.5f;

    [Header("Mira opcional no Player")]
    public Transform target;
    public bool seekPlayer = false;
    public float seekStrength = 2f;

    Rigidbody2D rb;
    bool isDead = false;              // trava contra múltiplos hits no mesmo frame

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 dir)
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        rb.linearVelocity = dir.normalized * speed;          // troque para rb.velocity se preferir
        rb.angularVelocity = Random.Range(-60f, 60f);
    }

    public void Hit()
    {
        if (isDead) return;           // evita “dividir” mais de uma vez
        isDead = true;

        if (size == Size.Large)      Split(mediumPrefab, Size.Medium);
        else if (size == Size.Medium) Split(smallPrefab, Size.Small);
        // size == Small -> apenas destrói

        Destroy(gameObject);
    }

    void Split(GameObject childPrefab, Size childSize)
    {
        if (!childPrefab) return;

        for (int i = 0; i < 2; i++)
        {
            Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
            var go = Instantiate(childPrefab, pos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
            var a = go.GetComponent<Asteroid>();
            if (a != null)
            {
                a.size = childSize;
                a.target = this.target;           // herda alvo/configurações
                a.seekPlayer = this.seekPlayer;
                a.seekStrength = this.seekStrength;
                a.Launch(Random.insideUnitCircle.normalized);
            }
        }
    }

    void Update()
    {
        ScreenWrap();

        // Mira suave opcional no player
        if (seekPlayer && target != null)
        {
            Vector2 to = (Vector2)(target.position - transform.position);
            if (to.sqrMagnitude > 0.01f)
            {
                float speed = Mathf.Clamp(rb.linearVelocity.magnitude, minSpeed, maxSpeed);
                Vector2 desired = to.normalized * speed;
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desired, seekStrength * Time.deltaTime);
            }
        }
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
