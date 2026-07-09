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

    #region ADDING/REMOVING

    // looking for a quantity of an item type
    // the cell index of 'data' doesn't matter here
    public bool HasItem(inv_itemstack data)
    {
        int sum = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemIndex == data.itemIndex)
            {
                sum += items[i].itemCount;
            }
        }

        if (sum >= data.itemCount)
        {
            return true;
        } else
        {
            return false;
        }
    }


    // removing a SPECIFIC ITEM from a cell
    
    // the item count matters here, if the count is less than the number in the cell we only remove that amt
    // of course if the count is the same we remove all of it
    public void RemoveItem(inv_itemstack data)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].cellIndex == data.cellIndex && items[i].itemIndex == data.itemIndex)
            {
                if (data.itemCount >= items[i].itemCount)
                {
                    items.RemoveAt(i);
                    return; // we're done, so we do this to avoid loop issues
                } else
                {
                    items[i].itemCount -= data.itemCount;
                    return; // same story, we're done
                }
            }
        }

        // TODO: only do this if the item was actually removed, not just partly removed
        UpdateSelectedCellsForItem(data, false);
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
        UpdateSelectedCellsForItem(data, true);
    }

    public void UpdateSelectedCellsForItem(inv_itemstack data, bool isPlaced) // either placed (set cells to taken) or removed (unset them)
    {
        for (int x = 0; x < data.extendHorizontal; x++)
        {
            for (int y = 0; y < data.extendVertical; y++)
            {
                int indexOffset = x + y * inventory_width;

                cellsTaken[data.cellIndex + indexOffset] = isPlaced;
            }
        }
    }

    #endregion
}
