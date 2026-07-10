using System;
using UnityEngine;

// plural, because you'll have many crates on you in addition to whatever you're interacting with
// so we have to deal with lots of inventories at the same time

public class ui_inventories : MonoBehaviour
{   
    private static ui_inventories _instance;

    public static ui_inventories Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
        ClearCursor();
    }

    public Transform t_inventoryContainer;
    public GameObject p_inventory;

    public ui_inventorycursor cursor;

    public void BuildMenus(Func<inv_inventorydata>[] sources)
    {
        for (int i = 0; i < sources.Length; i++)
        {
            // TODO: proper positioning

            GameObject g_newWidget = Instantiate(p_inventory, t_inventoryContainer);

            g_newWidget.GetComponent<ui_inventorywidget>().BuildMenu(sources[i]);
        }
    }

    void Update()
    {
        cursor.transform.position = Input.mousePosition + new Vector2(-ItemManager.rawInventoryCellSize / 2f, ItemManager.rawInventoryCellSize / 2f);
    }

    // ***
    // item taking/giving 
    // ***

    public bool IsHoldingItem()
    {
        return cursor.heldItem != null;
    }

    public void ClearCursor()
    {
        cursor.ClearItem();
    }
    public void GiveItemToCursor(inv_itemstack itemData)
    {
        cursor.SetItem(itemData);
    }
}
