using UnityEngine;

[System.Serializable]
public class crft_craftdata
{
    public string craft_name;
    public crft_genericpartdata[] parts;

    // TODO: storing part connections

    public crft_craftdata() {}

    public crft_craftdata(string craft_name, crft_genericpartdata[] parts)
    {
        this.craft_name = craft_name;
        this.parts = parts;
    }
}
