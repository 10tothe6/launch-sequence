using UnityEngine;

// only runs in the cb editor

// fundamentally, this is just an orbiting camera

public class cam_bodyeditor : MonoBehaviour
{
    public float cameraRotateSpeed;
    public float scrollSpeed;

    public float viewDist;

    void Start()
    {
        CameraController.Instance.onChangeControlMode.AddListener(ProcessChangeInControlMode);
        CameraController.Instance.onCameraUpdate.AddListener(CameraUpdate);
    }

    public void ProcessChangeInControlMode()
    {
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.BodyEditor)
        {
            EnterControl();
        } else {ExitControl();}
    }

    public void EnterControl()
    {
        if (viewDist == 0)
        {
            viewDist = 200;
        }
    }
    public void ExitControl()
    {
        
    }

    void CameraUpdate()
    {
        if (CameraController.controlMode == (ushort)CameraControlMode.BodyEditor)
        {

            if (Input.mouseButtonRight)
            {
                transform.Rotate(Vector3.up * Input.mouseMovement.x * cameraRotateSpeed + transform.right * Input.mouseMovement.y * -cameraRotateSpeed, Space.World);
            }

            // the body is always located at (0,0,0) so we don't need a ref
            transform.position = -transform.forward * viewDist;

            viewDist += Input.scrollWheelAxis * scrollSpeed;
        }
    }
}
