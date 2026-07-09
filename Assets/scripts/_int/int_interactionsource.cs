using UnityEngine;

public class int_interactionsource : MonoBehaviour
{
    public Transform src;

    private bool isDraggingObject;
    private Rigidbody objectToDrag;

    public float draggingDistance = 0.1f;
    public float dragForce = 10f;

    void Update()
    {
        if (isDraggingObject)
        {
            objectToDrag.linearVelocity = (src.position + src.forward * draggingDistance - objectToDrag.transform.position) * dragForce;
        }
    }

    public void StartDraggingObject(GameObject obj)
    {
        if (isDraggingObject) {return;} // don't want to call repeatedly

        isDraggingObject = true;
        
        if (obj.GetComponent<crft_genericpart>() != null)
        {
            objectToDrag = obj.transform.parent.parent.GetComponent<Rigidbody>();
        } else
        {
            objectToDrag = obj.GetComponent<Rigidbody>();
        }

        if (objectToDrag == null) {return;}
    
        if (objectToDrag.GetComponent<e_applyphysics>() != null)
        {
            objectToDrag.GetComponent<e_applyphysics>().useGravity = false;
        }
    }

    public void StopDraggingObject()
    {
        if (objectToDrag == null) {return;}
        if (!isDraggingObject) {return;}
        
        if (objectToDrag.GetComponent<e_applyphysics>() != null)
        {
            objectToDrag.GetComponent<e_applyphysics>().useGravity = true;
        }

        isDraggingObject = false;
        objectToDrag = null;
    }
}
