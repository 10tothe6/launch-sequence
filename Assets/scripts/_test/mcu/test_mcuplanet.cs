using UnityEngine;

public class test_mcuplanet : MonoBehaviour
{
    public int res;
    public float scl;

    void Start()
    {
        GetComponent<cbt_marchedchunk>().InitializeDirect(res, scl);
    }
}
