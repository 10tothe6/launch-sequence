using UnityEngine;

public class test_drawmeshbody : MonoBehaviour
{
    private cbt_meshbody body;

    [Header("CONFIG")]
    public bool runOnStart;

    void Start()
    {
        if (runOnStart)
        {
            Draw();
        }
    }
    
    public void Draw()
    {
        body = GetComponent<cbt_meshbody>();
        body.Initialize();
    }
}
