using System.Collections.Generic;
using Unity.VisualScripting;
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

    public inv_itemstack GetItemAtCell(int cellIndex)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].cellIndex == cellIndex)
            {
                return items[i];
            }
        }

        return null;
    }

    // adding an item to the inventory
    public void AddItem(inv_itemstack data)
    {
        // there are two possibilities here, either we combine the stack with another of the same type or we add it
        // if neither, then we return
        if (cellsTaken[data.cellIndex])
        {
            if (GetItemAtCell(data.cellIndex).itemIndex == data.itemIndex)
            {
                GetItemAtCell(data.cellIndex).itemCount = Mathf.Min(GetItemAtCell(data.cellIndex).GetData().stackSize, GetItemAtCell(data.cellIndex).itemCount + data.itemCount);
            } else
            {
                return; // can't add
            }
        } else
        {
            items.Add(data);
        }

        // no need to refresh the whole array here, just need to set the cells for this one item
    }
}
