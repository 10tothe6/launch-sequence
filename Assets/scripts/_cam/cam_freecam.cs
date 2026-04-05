using UnityEngine;

public class cam_freecam : MonoBehaviour
{
    public Transform t_player;
    public LayerMask cullingMask;

    public float moveSpeed;

    private Quaternion storedRotation;



    void Start()
    {
        CameraController.Instance.onChangeControlMode.AddListener(ProcessChangeInControlMode);
        CameraController.Instance.onCameraUpdate.AddListener(CameraUpdate);

        storedRotation = Quaternion.identity;
    }

    public void ProcessChangeInControlMode()
    {
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.Freecam)
        {
            EnterControl();
        } else {ExitControl();}
    }

    public void EnterControl()
    {
        CameraController.cam_main.cullingMask = cullingMask;
        transform.SetParent(t_player);
        
        CameraController.ZeroOut();

        if (storedRotation != Quaternion.identity) {LocalPlayer.Instance.transform.rotation = storedRotation;}
        CameraController.t_cam.parent.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public void ExitControl()
    {
        storedRotation = LocalPlayer.Instance.transform.rotation;
    }

    void CameraUpdate()
    {
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.Freecam)
        {
            if (Input.mouseButtonRight)
            {
                t_player.transform.Rotate(Vector3.up * -Input.mouseMovement.x + transform.right * Input.mouseMovement.y, Space.World);
            }

            // TODO: run the position changes THROUGH THE RENDERING MANAGER!
            LocalPlayer.Instance.MoveBy(
            (transform.forward * Input.inputAxisForward +
            transform.right * Input.inputAxisHorizontal + 
            transform.up * Input.inputAxisVertical) * moveSpeed);
        }
    }
}
