using UnityEngine;

// an item, in an inventory

[System.Serializable]
public class inv_itemstack
{
    // the basics, same as older inventory implementations
    public int itemIndex;
    public int itemCount;




    public int cellIndex; // where the "origin" of the item is

    // how many cells the item extends in either direction
    // this works with negative too (positive = right and up)
    public int extendHorizontal;
    public int extendVertical;

    public inv_itemstack() {}


    // assuming rotation is 0
    public inv_itemstack(int itemIndex, int itemCount, int cellIndex)
    {
        this.itemIndex = itemIndex;
        this.itemCount = itemCount;

        this.cellIndex = cellIndex;

        // filling out the extend horizontal and vertical based on the item's static data
        this.extendHorizontal = ItemManager.Instance.items[itemIndex].occupyWidth;
        this.extendVertical = ItemManager.Instance.items[itemIndex].occupyHeight;
    }

    public inv_itemstack(int itemIndex, int itemCount, int cellIndex, int rotationIndex)
    {
        this.itemIndex = itemIndex;
        this.itemCount = itemCount;

        this.cellIndex = cellIndex;

        // filling out the extend horizontal and vertical based on the item's static data
        this.extendHorizontal = ItemManager.Instance.items[itemIndex].occupyWidth;
        this.extendVertical = ItemManager.Instance.items[itemIndex].occupyHeight;
    }

    public inv_itemdata GetData()
    {
        return ItemManager.Instance.items[itemIndex];
    }
}
