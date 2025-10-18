using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 0f;               // <= 0 = sem expiração por tempo
    public float offscreenMargin = 0.05f; // destrói quando sai da tela

    float alive;

    void Update()
    {
        // Vida opcional por tempo
        if (life > 0f)
        {
            alive += Time.deltaTime;
            if (alive >= life) Destroy(gameObject);
        }

        // Mata fora da tela
        var cam = Camera.main;
        var v = cam.WorldToViewportPoint(transform.position);
        if (v.x < -offscreenMargin || v.x > 1f + offscreenMargin ||
            v.y < -offscreenMargin || v.y > 1f + offscreenMargin)
        {
            Destroy(gameObject);
        }
    }
}
