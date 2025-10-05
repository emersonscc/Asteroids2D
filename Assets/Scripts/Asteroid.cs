/*using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public enum Size { Large, Medium, Small }

    [Header("Config")]
    public Size size = Size.Large;
    public float minSpeed = 1.5f;
    public float maxSpeed = 3.5f;

    [Header("Prefabs para dividir")]
    public GameObject mediumPrefab; // preencha no Inspector do prefab Large
    public GameObject smallPrefab;  // preencha no Inspector do prefab Medium

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Chamada assim que o asteroide nasce
    public void Launch(Vector2 dir)
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        rb.linearVelocity = dir.normalized * speed;
        rb.angularVelocity = Random.Range(-60f, 60f);
    }

    // Quando levar tiro
    public void Hit()
    {
        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.AddScore(size == Size.Large ? 20 :
                        size == Size.Medium ? 50 : 100);
        }

        if (size == Size.Large) Split(mediumPrefab, Size.Medium);
        else if (size == Size.Medium) Split(smallPrefab, Size.Small);

        Destroy(gameObject);
    }

    private void Split(GameObject prefab, Size newSize)
    {
        if (prefab == null) return;

        for (int i = 0; i < 2; i++)
        {
            Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
            var child = Instantiate(prefab, pos, Quaternion.Euler(0, 0, Random.Range(0, 360f)));
            var a = child.GetComponent<Asteroid>();
            if (a != null)
            {
                a.size = newSize;
                a.Launch(Random.insideUnitCircle.normalized);
            }
        }
    }

    private void Update()
    {
        ScreenWrap();
    }

    private void ScreenWrap()
    {
        var cam = Camera.main;
        var view = cam.WorldToViewportPoint(transform.position);

        if (view.x > 1f) view.x = 0f; else if (view.x < 0f) view.x = 1f;
        if (view.y > 1f) view.y = 0f; else if (view.y < 0f) view.y = 1f;

        var world = cam.ViewportToWorldPoint(view);
        transform.position = new Vector3(world.x, world.y, 0f);
    }
}
*/
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public enum Size { Large, Medium, Small }

    [Header("Config")]
    public Size size = Size.Large;
    public float minSpeed = 1.5f;
    public float maxSpeed = 3.5f;

    [Header("Prefabs para dividir")]
    public GameObject mediumPrefab; // preencher no prefab Large
    public GameObject smallPrefab;  // preencher no prefab Medium

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void Launch(Vector2 dir)
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        rb.linearVelocity = dir.normalized * speed;
        rb.angularVelocity = Random.Range(-60f, 60f);
    }

    public void Hit()
    {
        // pontuação
        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
            gm.AddScore(size == Size.Large ? 20 : size == Size.Medium ? 50 : 100);

        // dividir
        if (size == Size.Large) Split(mediumPrefab, Size.Medium);
        else if (size == Size.Medium) Split(smallPrefab, Size.Small);

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
                a.Launch(Random.insideUnitCircle.normalized);
            }
        }
    }
}
