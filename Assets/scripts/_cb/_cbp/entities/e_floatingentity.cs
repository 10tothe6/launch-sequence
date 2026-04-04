using UnityEngine;

public class e_floatingentity : MonoBehaviour
{
    public e_floatingentitydata data;
    public bool setupOnAwake;

    void Awake()
    {
        if (setupOnAwake)
        {
            Setup();
        }
    }

    public void Setup()
    {
        data = new e_floatingentitydata(transform);
        data.defaultScale = 1;
    }
}
