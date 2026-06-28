using UnityEngine;

// attaching a label to a bounding box, in one of the corners

public enum ui_anchorcorner
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
}

public class ui_boundingboxlabel : MonoBehaviour
{
    public ui_anchorcorner position;

    public ui_boundingbox box;

    void Update()
    {
        if (position == ui_anchorcorner.BottomLeft)
        {
            transform.position = box.leftEdge.position - box.bounds.extents.y * Vector3.up;
        } 
        
        
        else if (position == ui_anchorcorner.BottomRight)
        {
            transform.position = box.rightEdge.position - box.bounds.extents.y * Vector3.up;
        }  
        
        
        else if (position == ui_anchorcorner.TopLeft)
        {
            transform.position = box.leftEdge.position + box.bounds.extents.y * Vector3.up;
        } 
        
        
        else if (position == ui_anchorcorner.TopRight)
        {
            transform.position = box.rightEdge.position + box.bounds.extents.y * Vector3.up;
        } 
    }
}
