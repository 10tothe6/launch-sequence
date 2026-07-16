using UnityEngine;


// an object that opens the ship editor,
// allowing you to build, edit and spawn different vehicles

public class crft_vehiclespawner : MonoBehaviour
{
    public Transform t_cameraPoint;
    
    public void OpenVehicleEditor()
    {
        UIManager.Instance.OpenVehicleEditor(this);
    }
}
