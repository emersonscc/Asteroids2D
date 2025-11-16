using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TimedSelfDestruct : MonoBehaviour
{
    public float lifetime = 1f;   // 1 segundo
    public bool fadeOut = true;

    SpriteRenderer sr;
    float t;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        t += Time.deltaTime;

        if (fadeOut && sr)
        {
            float a = Mathf.Clamp01(1f - t / lifetime);
            var c = sr.color; c.a = a; sr.color = c;
        }

        if (t >= lifetime)
            Destroy(gameObject);
    }
}
