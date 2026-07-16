using Unity.VisualScripting;
using UnityEngine;

public class cam_vehicleeditor : MonoBehaviour
{
    [Header("CAMERA TRANSFORM VARS")]
    public float cameraRotateSpeed;
    [HideInInspector]
    public float viewDist;
    public float scrollSpeed;

    private Transform t_cameraPivotPosition;



    void Start()
    {
        CameraController.Instance.onChangeControlMode.AddListener(ProcessChangeInControlMode);
        CameraController.Instance.onCameraUpdate.AddListener(CameraUpdate);
    }

    // ************************************
    public void ProcessChangeInControlMode()
    {
        if (CameraController.controlMode == (ushort)CameraControlMode.VehicleEditor)
        {
            EnterControl();
        } else if (CameraController.previousControlMode == (ushort)CameraControlMode.VehicleEditor){ExitControl();}
    }

    public void EnterControl()
    {
        transform.SetParent(UIManager.Instance.vehicleEditor.part_reference.t_cameraPoint);
        
        // need cursor for the editor
        Cursor.lockState = CursorLockMode.None;

        UIManager.Instance.LockPlayer();

        viewDist = 0.25f;
        
    }
    public void ExitControl()
    {
        
    }
    // ************************************

    // because Update() isn't rlly available for anyone except Program.cs
    void CameraUpdate()
    {
        // make sure the camera mode is right
        // otherwise we'd just be running this constantly
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.VehicleEditor)
        {
            if (Input.mouseButtonRight)
            {
                transform.Rotate(transform.parent.parent.up * Input.mouseMovement.x * cameraRotateSpeed, Space.World);
                transform.Rotate(Vector3.right * Input.mouseMovement.y * -cameraRotateSpeed, Space.Self);
            }

            // the body is always located at (0,0,0) so we don't need a ref
            transform.position = transform.parent.position -transform.forward * viewDist;

            viewDist += Input.scrollWheelAxis * -scrollSpeed;
        }
    }
}
