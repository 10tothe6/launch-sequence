using UnityEngine;

public class test_mcuchunkconsole : MonoBehaviour
{
    public cbt_marchedchunk comp;



    public bool split;

    void Update()
    {
        if (split)
        {
            split = false;
            comp.Subdivide();
        }
    }
}
