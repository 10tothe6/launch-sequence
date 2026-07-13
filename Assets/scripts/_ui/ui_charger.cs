using UnityEngine;

// controller of the menu that allows you to change the charger's mode

public class ui_charger : MonoBehaviour
{
    private crft_charger part_reference;

    public void SetPartReference(crft_charger part_reference)
    {
        this.part_reference = part_reference;
    }
}
