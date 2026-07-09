using UnityEngine;


// used in the main menu for the floating objects that represent the buttons
// that's about it, though

public class randomangularvelocity : MonoBehaviour
{
    public bool useLocalAxes;
    public SnapAxis rotationAxis;


    private Rigidbody rb;
    public float max;
    public float min;

    void Start()
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

        if (!useLocalAxes)
        {
            if (rotationAxis != SnapAxis.All && rotationAxis != SnapAxis.X) {x = 0;}
            if (rotationAxis != SnapAxis.All && rotationAxis != SnapAxis.Y) {y = 0;}
            if (rotationAxis != SnapAxis.All && rotationAxis != SnapAxis.Z) {z = 0;}

            rb.angularVelocity = new Vector3(x, y, z);
        }
        else
        {
            Vector3 v = new Vector3(x,y,z);

            if (rotationAxis != SnapAxis.All && rotationAxis != SnapAxis.X) {
                v -= Vector3.Project(v, transform.right);
            }
            if (rotationAxis != SnapAxis.All && rotationAxis != SnapAxis.Y) {
                v -= Vector3.Project(v, transform.up);
            }
            if (rotationAxis != SnapAxis.All && rotationAxis != SnapAxis.Z) {
                v -= Vector3.Project(v, transform.forward);
            }

            rb.angularVelocity = v;
        }

        
    }
}
