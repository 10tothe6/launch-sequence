using System.Runtime.CompilerServices;
using UnityEngine;

public class cbt_coloredmeshchunks : MonoBehaviour
{
    public bool refreshOnUpdate;
    public Color[] debugColors;
    private cbt_meshbody comp;

    void Awake()
    {
        comp = GetComponent<cbt_meshbody>();
    }

    void Update()
    {
        if (refreshOnUpdate)
        {
            RefreshColors();
        }
    }

    // go through all the chunks of the mesh body, and give them their corresponding color
    public void RefreshColors()
    {
        for (int i = 0; i < comp.chunks.Count; i++)
        {
            // TODO: figure out what order we want the indices in
            comp.chunks[i].SetDebugColor(debugColors[0]);
        }
    }
}
