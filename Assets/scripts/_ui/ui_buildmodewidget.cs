using UnityEngine;

// ***** INFO ABOUT THIS CLASS: *****
// the menu from which you can select different structures/parts to place in build mode

public class ui_buildmodewidget : MonoBehaviour
{
    ui_list parts_list;

    private void Update()
    {
        // no need for an extra boolean variable,
        // we simply check if the HUD object is active

        // if so, we know we're in build mode
        if (transform.parent.gameObject.activeInHierarchy)
        {
            // also, this is all being done locally (and that's not a temp decision)
            // because most of this interaction stuff doesn't need to be shared over network

            // it only becomes network-relevant once the player actually CLICKS to place a part
            // so why make life more complicated than it needs to be?

            // even if we have a preview that syncs with other clients, 
            // we can just do that through the entity system

            

        }
    }
}
