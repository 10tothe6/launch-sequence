using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class cb_solarsystemdata
{
    public List<cb_trackedbody> bodies;

    public int starCount;
    public List<int> starIndices;

    public int planetCount;
    public List<int> planetIndices;
}
