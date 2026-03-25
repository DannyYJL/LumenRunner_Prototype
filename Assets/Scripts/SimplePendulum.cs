using UnityEngine;

public class SimplePendulum : MonoBehaviour
{
    public float angleLimit = 45f;
    public float speed = 2.0f;
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * speed) * angleLimit;
        transform.rotation = startRotation * Quaternion.Euler(0, 0, angle);
    }
}