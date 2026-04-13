using UnityEngine;
using UnityEngine.InputSystem;

public class cam_freecam : MonoBehaviour
{
    public Transform t_player;
    public LayerMask cullingMask;

    public float moveSpeed;

    private Quaternion storedRotation;

    void Awake()
    {
        
    }

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
        if (LocalPlayer.localClient != null)
        {
            t_player = LocalPlayer.localClient.controllingEntity.transform;
        } else
        {
            cmd.Log("there was a problem entering freecam mode");
        }

        CameraController.cam_main.GetComponent<cbr_applyatmosphere>().enabled = true;

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

    // continuing the trend of avoiding Update()
    void CameraUpdate()
    {
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.Freecam && t_player != null)
        {
            if (Input.mouseButtonRight)
            {
                t_player.transform.Rotate(Vector3.up * -Input.mouseMovement.x + transform.right * Input.mouseMovement.y, Space.World);
            }

            LocalPlayer.Instance.MoveBy(
            (transform.forward * Input.inputAxisForward +
            transform.right * Input.inputAxisHorizontal + 
            transform.up * Input.inputAxisVertical) * moveSpeed);

            // temporary for spawning a robot in
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                
            }
        }
    }
}
