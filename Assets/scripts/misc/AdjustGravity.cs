using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AdjustGravity : MonoBehaviour
{
    public Vector3 grav;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    void FixedUpdate()
    {
        rb.linearVelocity += grav * Time.deltaTime;
    }
}
