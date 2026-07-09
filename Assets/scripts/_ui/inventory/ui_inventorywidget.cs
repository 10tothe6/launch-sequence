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
    private inv_inventorydata cachedSourceData;

    public Transform t_cellContainer;

    public GameObject p_cell;

    public float spaceBetweenCells;

    public void BuildMenu(Func<inv_inventorydata> source)
    {
        this.source = source;
        UpdateCachedData();


        BuildCellGrid();
    }

    private void BuildCellGrid()
    {
        float cellSize = p_cell.GetComponent<RectTransform>().sizeDelta.x; // can just use x or y cuz they're square

        Vector3 positionOffset = Vector3.zero;
        if (drawCentered)
        {
            positionOffset = new Vector3(-cachedSourceData.inventory_width / 2f * cellSize - (cachedSourceData.inventory_width-1)/ 2f * spaceBetweenCells, cachedSourceData.inventory_height / 2f * cellSize + (cachedSourceData.inventory_height-1)/ 2f * spaceBetweenCells, 0);
        }

        for (int x = 0; x < cachedSourceData.inventory_width; x++)
        {
            for (int y = 0; y < cachedSourceData.inventory_height; y++)
            {
                GameObject g_newCell = Instantiate(p_cell, t_cellContainer);

                // positioning the cell
                g_newCell.transform.localPosition = positionOffset + new Vector3(x * (cellSize + spaceBetweenCells), -y * (cellSize + spaceBetweenCells), 0);
            }
        }
    }

    void UpdateCachedData()
    {
        cachedSourceData = source.Invoke();
    }
}
