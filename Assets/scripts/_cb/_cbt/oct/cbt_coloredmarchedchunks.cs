using UnityEngine;

public class cbt_coloredmarchedchunks : MonoBehaviour
{
    public cbt_ChunkColoringMode coloringMode;

    public bool refreshOnUpdate;
    public Color[] debugColors;
    private cbt_marchedbody comp;

    void Awake()
    {
        comp = GetComponent<cbt_marchedbody>();
    }

    void Update()
    {
        if (refreshOnUpdate)
        {
            RefreshColors();
        }
    }

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
            // this concept does not exist
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
