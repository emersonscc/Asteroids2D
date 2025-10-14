using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Rotate the transform of the game object. This is attached to by
        // 45 degrees, taking into account the time elapsed since last frame.
        transform.Rotate(new Vector3 (0, 0, 45) * Time.deltaTime);
    }
}
