using System.Collections.Generic;
using UnityEngine;

// better organizing inventory data being sent over the network with this class

// appears on crft_inventory

[System.Serializable]
public class inv_inventorydata
{
    public int inventory_width;
    public int inventory_height;

    // inventory data is stored in 2 parts
    public bool[] cellsTaken;
    public List<inv_itemstack> items;

    public inv_inventorydata(){}

    public inv_inventorydata(int inventory_width, int inventory_height)
    {
        this.inventory_width = inventory_width;
        this.inventory_height = inventory_height;
    }

    public void UpdateInventoryData(List<inv_itemstack> newItemData)
    {
        
    }

    // adding an item to the inventory
    public void AddItem(inv_itemstack data)
    {
        
    }
}
