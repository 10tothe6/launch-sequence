using UnityEngine;

// only runs in the cb editor

public class cam_bodyeditor : MonoBehaviour
{
    void Start()
    {
        CameraController.Instance.onChangeControlMode.AddListener(ProcessChangeInControlMode);
        CameraController.Instance.onCameraUpdate.AddListener(CameraUpdate);
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
        
    }
    public void ExitControl()
    {
        
    }

    void CameraUpdate()
    {
        
    }
}
