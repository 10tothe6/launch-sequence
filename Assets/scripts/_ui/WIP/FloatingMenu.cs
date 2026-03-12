using UnityEngine;
using UnityEngine.Events;

// works along with ToolbarMenu.cs
public class FloatingMenu : MonoBehaviour
{
    public GameObject g_menuParent;
    public GameObject g_interactPoint;

    public bool isGrabbed;

    private Vector3 cursorOffset;

    public UnityAction handlePositioning;
    public UnityAction onGrab;

    void Awake()
    {
    }

    void Update()
    {
        if (Input.mouseButtonDownLeft && ui_canvasutils.IsCursorInteract(g_interactPoint, false))
        {
            isGrabbed = true;
            if (onGrab != null) onGrab.Invoke();

            cursorOffset = transform.position - new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);
        }
        if (!Input.mouseButtonDownLeft)
        {
            isGrabbed = false;
        }

        if (handlePositioning != null) handlePositioning.Invoke();

        if (isGrabbed)
        {
            transform.position = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0) + cursorOffset;
        }
    }
}
