using UnityEngine;
using System.Collections.Generic;

// any part that contains items will use this

public class crft_inventory : MonoBehaviour
{
    public inv_inventorydata data;

    void Awake()
    {
        InitializeInventoryData();
    }

    private void InitializeInventoryData()
    {
        data.cellsTaken = new bool[data.inventory_width * data.inventory_height];
        data.items = new List<inv_itemstack>();
    }

    public void OpenInventory()
    {
        UIManager.Instance.OpenInventory();
        ui_inventories.Instance.BuildMenus(new System.Func<inv_inventorydata>[] {() => {return data;}});
    }
}
