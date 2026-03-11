using UnityEngine;

public class test_fluidspawn : MonoBehaviour
{
    void Start()
    {
        GetComponent<fluid_volume>().Spawn(transform.position);
    }
}
