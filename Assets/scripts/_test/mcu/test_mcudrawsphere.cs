using UnityEngine;

public class test_mcudrawsphere : MonoBehaviour
{   
    public int resolution;
    public float actual_size;

    public float noise_freq;
    public float noise_amp;
    public bool regen;

    void Start()
    {
        GetComponent<mcu_drawmesh>().noise_amp = noise_amp;
        GetComponent<mcu_drawmesh>().noise_freq = noise_freq;
        GetComponent<mcu_drawmesh>().InitializeAsSphere(resolution, resolution, resolution, actual_size, actual_size, actual_size);
    }

    void Update()
    {
        if (regen)
        {
            regen = false;

            GetComponent<mcu_drawmesh>().noise_amp = noise_amp;
        GetComponent<mcu_drawmesh>().noise_freq = noise_freq;
            GetComponent<mcu_drawmesh>().InitializeAsSphere(resolution, resolution, resolution, actual_size, actual_size, actual_size);
        }
    }
}
