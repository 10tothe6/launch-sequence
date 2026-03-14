using UnityEngine;

[System.Serializable]
public class cb_trackedbodydata
{
    public string name;
    public ushort bodyType;
    
    public float mass;

    // quick-and-easy checks
    public bool hasSurface;
    public bool hasAtmosphere;

    public Color mapViewColor;
    public int iconIndex;

    // configs
    public cbp_config pConfig;
    public cbt_config tConfig;

    public cb_trackedbodydata()
    {
        pConfig = new cbp_config();
        tConfig = new cbt_config();
    }

}
