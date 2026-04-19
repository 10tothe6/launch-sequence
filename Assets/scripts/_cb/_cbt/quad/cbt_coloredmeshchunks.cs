using UnityEngine;

public enum cbt_ChunkColoringMode
{
    NONE, // just white
    LOD, // whatever the chunk LOD is
    PARENT_FACE, // whatever the parent face is
    RAINBOW, // i was bored, okay?
}

public class cbt_coloredmeshchunks : MonoBehaviour
{
    public cbt_ChunkColoringMode coloringMode;

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
        if (coloringMode == cbt_ChunkColoringMode.LOD)
        {
            for (int i = 0; i < comp.chunks.Count; i++)
            {
                // TODO: figure out what order we want the indices in
                comp.chunks[i].SetDebugColor(debugColors[comp.detailLevelThresholds.Length - 1 -comp.chunks[i].levelOfDetail]);
            }
        } else if (coloringMode == cbt_ChunkColoringMode.PARENT_FACE)
        {
            for (int i = 0; i < comp.chunks.Count; i++)
            {
                // TODO: figure out what order we want the indices in
                comp.chunks[i].SetDebugColor(debugColors[comp.chunks[i].startingFace]);
            }
        } else if (coloringMode == cbt_ChunkColoringMode.NONE)
        {
            for (int i = 0; i < comp.chunks.Count; i++)
            {
                // TODO: figure out what order we want the indices in
                comp.chunks[i].SetDebugColor(Color.white);
            }
        } else if (coloringMode == cbt_ChunkColoringMode.RAINBOW)
        {
            for (int i = 0; i < comp.chunks.Count; i++)
            {
                // TODO: figure out what order we want the indices in
                comp.chunks[i].SetDebugColor(util_misc.RainbowColor(0.75f));
            }
        }
    }
}
