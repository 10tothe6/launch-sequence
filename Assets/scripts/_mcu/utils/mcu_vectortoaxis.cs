using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class mcu_vectortoaxis
{
    public Vector3 vector;
    public List<int> moves;
    public List<bool> directions;

    public mcu_vectortoaxis() {}

    public mcu_vectortoaxis(Vector3 vector, List<int> moves, List<bool> directions)
    {
        this.vector = vector;
        this.moves = moves;
        this.directions = directions;
    }
}
