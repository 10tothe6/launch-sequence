using UnityEngine;

public class test_mcuplanet : MonoBehaviour
{
    public cbt_marchedchunk comp;


    public int res;
    public float scl;

    void Start()
    {
        comp.InitializeDirect(res, scl);
    }
}
