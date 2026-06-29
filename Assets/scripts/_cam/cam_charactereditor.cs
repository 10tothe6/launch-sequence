using UnityEngine;

// the camera controls for when the player is "building onto" their character

public class cam_charactereditor : MonoBehaviour
{
    public void ProcessChangeInControlMode()
    {
        if (CameraController.controlMode == (ushort)CameraControlMode.PlayerFirstPerson)
        {
            EnterControl();
        } else if (CameraController.previousControlMode == (ushort)CameraControlMode.PlayerFirstPerson){ExitControl();}
    }

    public void EnterControl()
    {
        
    }


    public void ExitControl()
    {
        
    }
}
