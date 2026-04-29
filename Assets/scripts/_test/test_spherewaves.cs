using UnityEngine;

public class test_spherewaves : MonoBehaviour
{
    public Vector4[] waveDirs;
    public Material m;
    public Transform[] t_targets;
    public Transform t_targetParent;

    void Awake()
    {
        waveDirs = new Vector4[t_targetParent.childCount];
        t_targets = new Transform[t_targetParent.childCount];
        for (int i = 0; i < t_targetParent.childCount; i++)
        {
            t_targets[i] = t_targetParent.GetChild(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < t_targets.Length; i++)
        {
            waveDirs[i] = t_targets[i].position.normalized;
        }

        m.SetVectorArray("waveVectors", waveDirs);
    }
}
