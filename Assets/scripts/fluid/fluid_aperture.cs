using System.Collections.Generic;
using UnityEngine;

// rather than the volume script handling the movement, apertures do all that themselves
// keeps things simpler

// TODO: have the y-position of the aperture itself matter

public class fluid_aperture : MonoBehaviour
{
    public float flowRate; // in m^3 per second

    public bool isOpen;

    public List<fluid_volume> connections;

    void Update()
    {
        if (isOpen)
        {
            float heightA = connections[0].GetGlobalFluidHeight();
            float heightB = connections[1].GetGlobalFluidHeight();
            if (heightA > heightB)
            {
                connections[0].ModifyVolume(-flowRate * Time.deltaTime);
                connections[1].ModifyVolume(flowRate * Time.deltaTime);
            } else
            {
                connections[0].ModifyVolume(flowRate * Time.deltaTime);
                connections[1].ModifyVolume(-flowRate * Time.deltaTime);
            }
        }
    }
}
