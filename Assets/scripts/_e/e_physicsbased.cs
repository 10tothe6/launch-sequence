using UnityEngine;


// makes sure that the entity position is updated from the rigidbody
// this is on ALL PHYSICALLY BASED ENTITIES, INCLUDING THE PLAYER

public class e_physicsbased : MonoBehaviour
{
    private e_genericentity eComp;

    void Awake()
    {
        eComp = GetComponent<e_genericentity>();
        eComp.data.isPhysicsBased = true; // in case it wasn't already
        eComp.onEntityUpdate.AddListener(EntityUpdate);
    }

    void EntityUpdate()
    {
        // updating the entity position from the rigidbody,
        // based on where we are relative to the unity origin
        // (and where the unity origin is in game space)

        num_precisevector3 gameSpacePosition = Coord.originPosition.Add(new num_precisevector3(transform.position));
        eComp.data.SetPosition(gameSpacePosition);
    }
}
