using UnityEngine;

public class cam_mainmenu : MonoBehaviour
{
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
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.MainMenu)
        {
            EnterControl();
        } else {ExitControl();}
    }

    public void EnterControl()
    {
        CameraController.Instance.transform.position = new Vector3(14.2299995f,-4.5999999f,56.6199989f);
        CameraController.Instance.transform.forward = -Vector3.forward;
    }
    public void ExitControl()
    {
        
    }

    void CameraUpdate()
    {
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.MainMenu)
        {
            CameraController.Instance.transform.Rotate(Vector3.forward * Time.deltaTime * cb_mainmenucontroller.Instance.camRotSpeed, Space.Self);
        }
    }
}
