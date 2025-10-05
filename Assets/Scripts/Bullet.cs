/*using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 1.5f;

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
        ScreenWrap();
    }

    void ScreenWrap()
    {
        var cam = Camera.main;
        var view = cam.WorldToViewportPoint(transform.position);

        bool wrapped = false;
        if (view.x > 1) { view.x = 0; wrapped = true; }
        else if (view.x < 0) { view.x = 1; wrapped = true; }
        if (view.y > 1) { view.y = 0; wrapped = true; }
        else if (view.y < 0) { view.y = 1; wrapped = true; }

        if (wrapped) transform.position = cam.ViewportToWorldPoint(view);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            other.GetComponent<Asteroid>().Hit();
            Destroy(gameObject);
        }
    }
}*/

using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 1.5f;

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    // A bala usa Collider2D com "Is Trigger" marcado
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Asteroid>(out var a))
        {
            a.Hit();
            Destroy(gameObject);
        }
    }
}

