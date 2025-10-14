using UnityEngine;

public class BulletLife : MonoBehaviour
{
    public float offscreenMargin = 0.05f; // mata fora da tela

    void Update()
    {
        var cam = Camera.main;
        var v = cam.WorldToViewportPoint(transform.position);
        if (v.x < -offscreenMargin || v.x > 1f + offscreenMargin ||
            v.y < -offscreenMargin || v.y > 1f + offscreenMargin)
        {
            Destroy(gameObject);
        }
    }
}
