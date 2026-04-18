using UnityEngine;

public class FreecamController : MonoBehaviour
{
    private e_genericentity entityData;
    void Awake()
    {
        entityData = GetComponent<e_genericentity>();

        entityData.onEnterControl.AddListener(EnterControl);
        entityData.onExitControl.AddListener(ExitControl);
    }

    void EnterControl()
    {
        CameraController.SetControlMode(CameraControlMode.Freecam);
    }

    void ExitControl()
    {
        
    }
}
