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

    public void GiveItemToCursor(inv_itemstack itemData)
    {
        if (cursor.heldItem != null)
        {
            return;
        }

        cursor.GrabItem(itemData);
    }
}
