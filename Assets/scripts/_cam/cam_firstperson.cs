using Unity.VisualScripting;
using UnityEngine;

public class cam_firstperson : MonoBehaviour
{
    public LayerMask cullingMask; 

    public Transform t_controlling;
    public PlayerController controller;

    void Start()
    {
        CameraController.Instance.onChangeControlMode.AddListener(ProcessChangeInControlMode);
        CameraController.Instance.onCameraUpdate.AddListener(CameraUpdate);
    }

    // ************************************
    public void ProcessChangeInControlMode()
    {
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.PlayerFirstPerson)
        {
            EnterControl();
        } else {ExitControl();}
    }

    public void SetControllingObject(GameObject g_toControl)
    {
        t_controlling = g_toControl.transform;
        controller = g_toControl.GetComponent<PlayerController>();

        g_toControl.GetComponent<PlayerController>().EnterControl();
    }

    public void EnterControl()
    {
        SetControllingObject(LocalPlayer.localClient.controllingEntity.gameObject);
        
        CameraController.cam_main.cullingMask = cullingMask;
        transform.SetParent(t_controlling.GetChild(0));
        
        CameraController.ZeroOut();
        
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
        if (CameraController.Instance.ins_controlMode == (ushort)CameraControlMode.PlayerFirstPerson)
        {
            player_keypresspacket toSend = LocalPlayer.GetKeypressPacket();
            // this ends up doing most of the work
            // really more like all of it
            controller.SetKeypresses(toSend);
        }
    }
}
