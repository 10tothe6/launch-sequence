using UnityEngine;

public class ui_snappable : MonoBehaviour
{
    public Transform[] snappingPoints;

    private bool isHolding;
    private Vector3 targetPos;
    public float handleMoveSpeed;

    void Awake()
    {
        targetPos = transform.position;
    }

    private void Update()
    {
        if (Input.mouseButtonDownLeft && ui_canvasutils.IsCursorInteract(gameObject, true))
        {
            isHolding = true;
        }
        
        if (!Input.mouseButtonLeft)
        {
            isHolding = false;
        }

        if (isHolding)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

            // basically: teleport the handle to the nearest snapping point
            int kingIndex = 0;
            float kingDist = 9999;
            for (int i = 0; i < snappingPoints.Length; i++)
            {
                if (Vector3.Distance(mousePos, snappingPoints[i].position) < kingDist)
                {
                    kingIndex = i;
                    kingDist = Vector3.Distance(mousePos, snappingPoints[i].position);
                }
            }

            Vector3 drag = (mousePos - snappingPoints[kingIndex].position);
            drag -= Vector3.Project(drag, Vector3.up);
            targetPos = snappingPoints[kingIndex].position +  drag* 0.25f;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, handleMoveSpeed);
    }
}
