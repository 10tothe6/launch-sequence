using UnityEngine;

public class test_precision : MonoBehaviour
{
    public float a;
    public float b;

    void Start()
    {
        num_precise _a = new num_precise(a);
        num_precise _b = new num_precise(b);

        num_precise _c = _a.Sub(_b);

        Debug.Log(_c.AsDouble());
    }
}
