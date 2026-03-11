using UnityEngine;

[System.Serializable]
public class cb_trackedbodydata
{
    public string name;
    public ushort bodyType;
    
    public float mass;

    public bool hasSurface;

    public Color mapViewColor;
    public int iconIndex;

    // configs
    public cbp_config pConfig;
    public cbt_config tConfig;

}
