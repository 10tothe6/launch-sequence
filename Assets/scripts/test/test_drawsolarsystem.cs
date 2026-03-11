using UnityEngine;

public class test_drawsolarsystem : MonoBehaviour
{
    public cb_solarsystem ss;
    [Header("CONFIG")]
    public bool regenerate;

    void Start()
    {
        DrawSolarSystem();
    }

    void DrawSolarSystem()
    {
        ss.Generate();
        for (int i = 0; i < ss.monoBodies.Count; i++)
        {
            if (ss.monoBodies[i].data.bodyType == 0) {ss.monoBodies[i].transform.position = Vector3.zero; continue;}
            ss.monoBodies[i].transform.position = 
            ss.monoBodies[ss.monoBodies[i].data.pConfig.parentIndex].data.pConfig.iPosition + 
            ss.monoBodies[i].data.pConfig.iPosition;
        }
    }

    void Update()
    {
        if (regenerate)
        {
            regenerate = false;
            DrawSolarSystem();
        }
    }
}
