using UnityEngine;


// NEEDS AN e_physicsbased ALSO

// this class is specifically for entities that don't have other scripts to do their physics for them

public class e_applyphysics : MonoBehaviour
{
    public bool useGravity = true;
    public e_genericentity eComp;
    private Rigidbody rb;
    public Vector3 gravityDirection = -Vector3.right;
    public float gravitationalAcceleration = 0.981f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        eComp = GetComponent<e_genericentity>();

        eComp.onEntityUpdate.AddListener(EntityUpdate);


        // just making sure:
        rb.useGravity = false;
        rb.angularDamping = 0;
    }

    void EntityUpdate()
    {
        if (rb.isKinematic) {return;}
        
        if (useGravity)
        {
            rb.linearVelocity += gravityDirection * gravitationalAcceleration * Time.deltaTime;
        }
    }
}
