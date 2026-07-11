using UnityEngine;

public class ui_charactereditor : MonoBehaviour
{
    // for checking whether the player is trying to place an item
    public ui_inventorycursor inventoryCursor;

    public GameObject g_playerBounds;

    public bool isActive;

    private inv_itemstack itemCache;
    private GameObject g_previewPart;

    public LayerMask whatIsPlaceable;

    void Update()
    {
        if (isActive)
        {
            if (ui_canvasutils.IsCursorInBounds(g_playerBounds, false)) // are we hovering over the player?
            {
                if (inventoryCursor.heldItem != null)
                {
                    // the player is hovering an item over the player, so we take the item from the cursor,
                    //  and spawn in a part so they can see where they're placing
                    itemCache = new inv_itemstack(inventoryCursor.heldItem);
                    ui_inventories.Instance.ClearCursor();

                    // just add a part to the player's craft, and keep a reference so that we can
                   g_previewPart = LocalPlayer.localClient.controllingEntity.GetComponent<e_craft>().AddPart(itemCache.GetData().item_name);
                } else if (g_previewPart != null)
                {
                    RaycastHit hit;

                    // the 1f should be replaced with the viewdistance from the camera script
                    if (util_physics.MouseRaycast(out hit, 10f, whatIsPlaceable))
                    {
                        g_previewPart.transform.position = hit.point;
                        

                        if (Input.mouseButtonDownLeft)
                        {
                            // clicking places the part, we can just clear all the data and we good
                            itemCache = null;
                            g_previewPart = null;
                        }
                    }
                }
            }
        }
    }
}
