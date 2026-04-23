using UnityEngine;


// makes sure that the entity position is updated from the rigidbody

public class e_physicsobject : MonoBehaviour
{
    public Vector3 shoveFactor; // for use ONLY IN THE MAIN GAME
    private e_genericentity entityData;
    private Vector3 oldPosition;

    void Awake()
    {
        entityData = GetComponent<e_genericentity>();
    }

    void Update()
    {
        // updating the entity position from the rigidbody
        entityData.data.SetPosition(entityData.data.localPosition.Add(new num_precisevector3(transform.position - oldPosition -shoveFactor)));
        shoveFactor = Vector3.zero;
        oldPosition = transform.position;
    }
}
