using UnityEngine;

public enum net_gamemode
{
    Sandbox,
    Survival,
    Hardcore,
}

// data you need to host a server, 
// assembled through the 'host' menu (obv)

[System.Serializable]
public class net_serverhostingdata
{
    public string server_name;

    public ushort net_gamemode;
    public ushort bot_count; // only if permadeath

    public ushort max_player_count;

    // mission visual stuff
    public Texture2D flag;
    public Color col1; 

    

    public net_serverhostingdata() {}
}
