using UnityEngine;

public class test_rotate : MonoBehaviour
{
    public float speed;

    void FixedUpdate()
    {
        transform.Rotate(transform.forward * speed);
    }
}
