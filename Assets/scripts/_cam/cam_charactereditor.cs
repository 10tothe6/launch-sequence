using UnityEngine;

// the camera controls for when the player is "building onto" their character

public class cam_charactereditor : MonoBehaviour
{


    public float cameraRotateSpeed;
    [HideInInspector]
    public float viewDist;
    public float scrollSpeed;



    void Awake()
    {
        
    }

    void Start()
    {
        CameraController.Instance.onChangeControlMode.AddListener(ProcessChangeInControlMode);
        CameraController.Instance.onCameraUpdate.AddListener(CameraUpdate);
    }

    public void ProcessChangeInControlMode()
    {
        if (CameraController.controlMode == (ushort)CameraControlMode.CharacterEditor)
        {
            EnterControl();
        } else if (CameraController.previousControlMode == (ushort)CameraControlMode.CharacterEditor){ExitControl();}
    }

    public void EnterControl()
    {
        // (keeping the camera as a child of the player)

        transform.parent.localEulerAngles = Vector3.zero;
        transform.localPosition = Vector3.forward;
        transform.forward = -transform.localPosition;
        transform.Rotate(Vector3.forward * 90, Space.Self);

        UIManager.Instance.LockPlayer();

        viewDist = 0.25f;
    }


    public void ExitControl()
    {
        UIManager.Instance.UnlockPlayer();
    }

    void CameraUpdate()
    {
        if (CameraController.controlMode == (ushort)CameraControlMode.CharacterEditor)
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
