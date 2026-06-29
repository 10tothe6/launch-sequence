using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this is used when actually displaying an inventory


// this script is NOT featured on craft parts, 
// because their inventory only needs to be displayed when you "open" them
// in which case copies of this class are created as necessary

public class inv_inventory : MonoBehaviour
{
    public Func<inv_inventorydata> source;
    
    public UnityAction<List<inv_iteminstance>> onUpdateInventoryData;
}
