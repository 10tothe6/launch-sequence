using UnityEngine;

// an item, in an inventory

[System.Serializable]
public class inv_iteminstance
{
    public int cellIndex; // where the "origin" of the item is

    // how many cells the item extends in either direction
    // this works with negative too (positive = right and up)
    public int extendHorizontal;
    public int extendVertical;
}
