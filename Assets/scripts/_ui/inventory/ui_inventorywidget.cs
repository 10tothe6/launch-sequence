using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this is used when actually displaying an inventory


// this script is NOT featured on craft parts, 
// because their inventory only needs to be displayed when you "open" them
// in which case copies of this class are created as necessary

public class ui_inventorywidget : MonoBehaviour
{
    public bool drawCentered;


    public Func<inv_inventorydata> source;
    public inv_inventorydata cachedSourceData;

    public Transform t_cellContainer;

    public GameObject p_cell;

    public Transform t_itemIconContainer;

    //public float spaceBetweenCells;

    public void BuildMenu(Func<inv_inventorydata> source)
    {
        this.source = source;
        UpdateCachedData();


        BuildCellGrid();

        DrawItems();
    }

    private void DrawItems()
    {
        for (int i = 0; i < cachedSourceData.items.Count; i++)
        {
            t_cellContainer.GetChild(cachedSourceData.items[i].cellIndex).GetComponent<ui_inventoryslot>().SetItem(cachedSourceData.items[i]);
        }
    }

    private void BuildCellGrid()
    {
        float spaceBetweenCells = 0; // has to be zero, really
        float cellSize = p_cell.GetComponent<RectTransform>().sizeDelta.x; // can just use x or y cuz they're square

        Vector3 positionOffset = Vector3.zero;
        if (drawCentered)
        {
            positionOffset = new Vector3(-cachedSourceData.inventory_width / 2f * cellSize - (cachedSourceData.inventory_width-1)/ 2f * spaceBetweenCells, cachedSourceData.inventory_height / 2f * cellSize + (cachedSourceData.inventory_height-1)/ 2f * spaceBetweenCells, 0);
        }

        for (int y = 0, i = 0; y < cachedSourceData.inventory_height; y++)
        {
            for (int x = 0; x < cachedSourceData.inventory_width; x++, i++)
            {
                GameObject g_newCell = Instantiate(p_cell, t_cellContainer);

                // positioning the cell
                g_newCell.transform.localPosition = positionOffset + new Vector3(x * (cellSize + spaceBetweenCells), -y * (cellSize + spaceBetweenCells), 0);
                g_newCell.GetComponent<ui_inventoryslot>().Initialize(t_itemIconContainer, this, i);
            }
        }
    }

    void UpdateCachedData()
    {
        cachedSourceData = source.Invoke();
    }

    void RefreshWidget()
    {
        for (int i = 0; i < t_itemIconContainer.childCount; i++)
        {
            t_cellContainer.GetChild(i).GetComponent<ui_inventoryslot>().SetItem(null);
        }

        DrawItems();
    }



    // if not right click, then left click
    public void HandleInteractionAtCell(int cellIndex, bool isRightClick)
    {
        // remember, we only update the data once an item has been PLACED, not removed
        // and the update is done through  an inv_inventorytransfer, NEVER here

        if (cachedSourceData.cellsTaken[cellIndex])
        {
            
            if (ui_inventories.Instance.IsHoldingItem())
            {
                
            } else
            {
                
                if (isRightClick)
                {
                    
                } else
                {
                    
                    inv_itemstack data = cachedSourceData.GetItemAtCell(cellIndex);
                    ui_inventories.Instance.GiveItemToCursor(data);
                    cachedSourceData.RemoveItem(data);

                    RefreshWidget();
                }
            }



        } else
        {
            if (ui_inventories.Instance.IsHoldingItem())
            {
                
            } else
            {
                
            }
        }
    }
}
