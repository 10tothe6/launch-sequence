using System;
using System.Collections.Generic;
using UnityEngine;

// a string of connected parts in a structure
[System.Serializable]
public class strc_path
{
    public List<strc_part> path;

    public strc_path() {}

    public strc_path(List<strc_part> path)
    {
        this.path = path;
    }
}
