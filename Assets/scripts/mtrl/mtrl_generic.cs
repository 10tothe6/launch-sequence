using UnityEngine;

public enum mtrl_resourcetype
{
    Solid,
    Liquid,
    Gas,
    Plasma, // you never know...
}

[System.Serializable]
public class mtrl_generic
{
    public string name;
    public ushort type; // use the enum for referencing, please
    public SerializableColor color; // has to be serializable so we can write to disk

    public mtrl_generic() { }

    public mtrl_generic(string name, mtrl_resourcetype type)
    {
        this.name = name;
        this.type = (ushort)type;
        this.color = SerializableColor.white;
    }

    // public static mtrl_generic Get(string name)
    // {
    //     for (int i = 0; i < Sys.resources.Length; i++)
    //     {
    //         if (Sys.resources[i].name == name)
    //         {
    //             return Sys.resources[i];
    //         }
    //     }

    //     return null;
    // }
}
