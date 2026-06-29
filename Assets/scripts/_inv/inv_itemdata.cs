using UnityEngine;


[System.Serializable]
public class inv_itemdata
{
    public bool isPart; // whether the item should be spawned in as a spacecraft part, or not
    public string item_name;

    // items can be rotated, keep in mind
    public int occupyWidth;
    public int occupyHeight;

    public inv_itemdata() {}

    public inv_itemdata(int occupyWidth, int occupyHeight)
    {
        this.occupyWidth = occupyWidth;
        this.occupyHeight = occupyHeight;
    }
}
