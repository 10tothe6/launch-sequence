using UnityEngine;
using UnityEngine.InputSystem;

public class cam_mapview : MonoBehaviour
{
    public float viewDistance;
    public float viewDistanceSensitivity;

    void Update()
    {
        // this script only does anything if the camera is in map view mode
        if (CameraController.controlMode == (ushort)CameraControlMode.MapView)
        {
            Vector3 basePosition = WorldManager.Instance.t_mapBodyContainer.GetChild(
                WorldManager.Instance.mapFocusIndex
            ).position;

            // user input stuff is gonna be done on some cam scripts too
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                WorldManager.Instance.CycleMapFocus();
            }

            // rotating the camera with right-mouse
            if (Input.mouseButtonRight)
            {
                transform.Rotate(Vector3.up * Input.mouseMovement.x + transform.right * -Input.mouseMovement.y, Space.World);
            }

            viewDistance += Input.scrollWheelAxis * viewDistanceSensitivity;

            CameraController.t_cam.position = basePosition - transform.forward * viewDistance;
        }
    }
}
