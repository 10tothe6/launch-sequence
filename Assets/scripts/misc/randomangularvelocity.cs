using UnityEngine;


// used in the main menu for the floating objects that represent the buttons
// that's about it, though

public class randomangularvelocity : MonoBehaviour
{
    private Rigidbody rb;
    public float max;
    public float min;

    void Awake()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
        } else
        {
            return;
        }

        float x = Random.Range(-max, max);
        if (Mathf.Abs(x) < min) {x *= min / Mathf.Abs(x);}

        float y = Random.Range(-max, max);
        if (Mathf.Abs(y) < min) {y *= min / Mathf.Abs(y);}

        float z = Random.Range(-max, max);
        if (Mathf.Abs(z) < min) {z *= min / Mathf.Abs(z);}


        rb.angularVelocity = new Vector3(x, y, z);
    }
}
