using UnityEngine;
using UnityEngine.InputSystem;

// not actually an evolution of the supplyrun ItemDisplay.cs, though I could have build off of that
// this was one thing I just wanted to redo, cuz the old system wuz bad

public class ItemDisplay : MonoBehaviour
{
    void Update()
    {   
        // string heldItemName = "";
        // if (Player.inventory.items[Player.inventory.selectedCell] != null) heldItemName = Player.inventory.items[Player.inventory.selectedCell].GetName();

        // bool foundItem = false;
        // for (int i = 0; i < transform.childCount; i++)
        // {
        //     if (transform.GetChild(i).name == "i_" + heldItemName)
        //     {
        //         foundItem = true;
        //         transform.GetChild(i).gameObject.SetActive(true);
        //     } else {transform.GetChild(i).gameObject.SetActive(false);}
        // }

        // if (!foundItem && heldItemName.Length > 0)
        // {
        //     GameObject g_newItem = Instantiate(Player.inventory.items[Player.inventory.selectedCell].GetClass().p_item, transform);
        //     g_newItem.GetComponent<int_item>().SetAsVisualOnly();
        //     g_newItem.name = "i_" + heldItemName;

        //     g_newItem.transform.localPosition = Vector3.zero;
        // }

        // if (Keyboard.current.qKey.wasPressedThisFrame)
        // {
        //     DropHeldItem();
        // }
    }

    public void DropHeldItem()
    {
        DropHeldItem(Vector3.zero);
    }

    public void DropHeldItem(Vector3 force)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                //Player.inventory.RemoveHeldItem();
                // transform.GetChild(i).gameObject.GetComponent<int_item>().DisableVisualOnly();
                // transform.GetChild(i).gameObject.GetComponent<Rigidbody>().linearVelocity = force;
                // transform.GetChild(i).SetParent(null);
            }
        }
    }
}
