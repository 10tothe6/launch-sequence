using UnityEngine;

// plural, because you'll have many crates on you in addition to whatever you're interacting with
// so we have to deal with lots of inventories at the same time

public class ui_inventories : MonoBehaviour
{
    public Transform t_inventoryContainer;
    public GameObject p_inventory;

    public void Build()
    {
        if (LocalPlayer.localClient == null) {return;}
        if (LocalPlayer.localClient.controllingEntity == null) {return;}
        

        // first, get the inventories that the player has access to
        inv_inventorydata[] internalInventories = new inv_inventorydata[]{};

        for (int i = 0; i < internalInventories.Length; i++)
        {
            GameObject g_newInventory = Instantiate(p_inventory, t_inventoryContainer);

            // TODO: fix this reference
            ui_inventorywidget comp = g_newInventory.GetComponent<ui_inventorywidget>();
            comp.BuildMenu(() => {return internalInventories[i];});
        }
    }
}
