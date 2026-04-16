using UnityEngine;

public class test_drawmeshbody : MonoBehaviour
{
    private cbt_meshbody body;

    void Start()
    {
        body = GetComponent<cbt_meshbody>();
        body.Initialize();
    }
}
