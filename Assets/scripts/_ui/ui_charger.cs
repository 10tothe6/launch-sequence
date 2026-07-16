using UnityEngine;

// controller of the menu that allows you to change the charger's mode

public class ui_charger : MonoBehaviour
{
    private crft_charger part_reference;
    public ui_snappable mode_handle;

    public void SetPartReference(crft_charger part_reference)
    {
        this.part_reference = part_reference;

        mode_handle.SetSnappingPoint(part_reference.mode);
    }

    public void UpdateModeOnPart()
    {
        part_reference.mode = (ushort)mode_handle.current_index;
    }
}
