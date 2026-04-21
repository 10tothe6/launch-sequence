using UnityEngine;

public class vfx_rocketflamecontroller : MonoBehaviour
{
    [Range(0,1)]
    public float throttle;

    public float ambientAirPressure; // in kPa

    public bool isActive;
}
