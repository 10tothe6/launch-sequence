using UnityEngine;

// ***** INFO ABOUT THIS CLASS: *****
// the menu from which you can select different structures/parts to place in build mode

public class ui_buildmodewidget : MonoBehaviour
{
    public ui_list parts_list;
    public ui_snappable snappable;

    public void InitializeBuildHUD()
    {
        parts_list.SetItems(PartManager.Instance.GetBuildablePartNames());
        
        snappable.onChangeIndex.AddListener(SwitchToFromPartSelection);

        // show the menu, to start
        snappable.SetSnappingPoint(0);
    }

    public void SwitchToFromPartSelection(int snap_index)
    {
        if (snap_index == 0)
        {
            // parts list is open
            UIManager.Instance.LockPlayer();
        } else if (snap_index == 1)
        {
            // parts list is closed
            UIManager.Instance.UnlockPlayer();
        }
    }

    public void StartBuildingPart(string part_name)
    {
        // okay so basically the idea here is to spawn a part object (not as a proper entity, mind you)
        // and have it tinted
        // then we simply make it follow the player's mouse 
        // and attach to whatever you're looking at when you click


        // essentially just passing the word onto the player object
        LocalPlayer.localClient.controllingEntity.GetComponent<player_partmanager>().StartPlacingPart(part_name);
    }

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
