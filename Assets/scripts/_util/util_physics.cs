using UnityEngine;

public class util_physics
{
    public static bool MouseRaycast(out RaycastHit hit, float dist, LayerMask mask)
    {
        return Physics.Raycast(CameraController.t_cam.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist)) - CameraController.t_cam.position, out hit, Mathf.Infinity, mask);
    }
}
