using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class mtrl_generic
{
    public string name;
    public List<string> tags; // "fluid", "solid", etc.

    public SerializableColor color; // has to be serializable so we can write to disk

    public mtrl_generic() { }

    public mtrl_generic(string name)
    {
        this.name = name;
        this.color = SerializableColor.white;
        this.tags = new List<string>();
    }

    public mtrl_generic(string name, List<string> tags)
    {
        this.name = name;
        this.color = SerializableColor.white;
        this.tags = tags;
    }

    public mtrl_generic(string name, SerializableColor color)
    {
        this.name = name;
        this.color = color;
        this.tags = new List<string>();
    }

    public mtrl_generic(string name, SerializableColor color, List<string> tags)
    {
        this.name = name;
        this.color = color;
        this.tags = tags;
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
