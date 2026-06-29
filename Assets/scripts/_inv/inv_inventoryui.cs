using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this is used when actually displaying an inventory


// this script is NOT featured on craft parts, 
// because their inventory only needs to be displayed when you "open" them
// in which case copies of this class are created as necessary

public class inv_inventoryui : MonoBehaviour
{
    public Func<inv_inventorydata> source;
    private inv_inventorydata cachedSourceData;
    
    public UnityAction<List<inv_iteminstance>> onUpdateInventoryData;


    public void BuildMenu(Func<inv_inventorydata> source)
    {
        this.source = source;
        UpdateCachedData();


        
    }

    void UpdateCachedData()
    {
        cachedSourceData = source.Invoke();
    }
}
