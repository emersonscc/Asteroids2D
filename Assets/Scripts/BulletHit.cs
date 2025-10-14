using UnityEngine;

public class BulletHit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Asteroid>(out var a))
        {
            a.Hit();
            Destroy(gameObject); // bala se destrói ao acertar
        }
    }
}
