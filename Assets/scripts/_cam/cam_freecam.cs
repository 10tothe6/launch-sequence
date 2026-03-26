using UnityEngine;

public class cam_freecam : MonoBehaviour
{
    public Transform t_player;
    public LayerMask cullingMask;

    void Start()
    {
        CameraController.Instance.onChangeControlMode.AddListener(ProcessChangeInControlMode);
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
    }
    public void ExitControl()
    {
        
    }

    void Update()
    {
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.Freecam)
        {
            if (Input.mouseButtonRight)
            {
                t_player.transform.Rotate(Vector3.up * -Input.mouseMovement.x + transform.right * Input.mouseMovement.y, Space.World);
            }

            // TODO: run the position changes THROUGH THE RENDERING MANAGER!
            // t_player.transform.position += 
            // transform.forward * Input.inputAxisForward +
            // transform.right * Input.inputAxisHorizontal + 
            // transform.up * Input.inputAxisVertical;
        }
    }
}
