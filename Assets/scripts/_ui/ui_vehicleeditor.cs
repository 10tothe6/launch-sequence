using UnityEngine;

public class ui_vehicleeditor : MonoBehaviour
{
    [HideInInspector]
    public crft_vehiclespawner part_reference;

    public void SetPartReference(crft_vehiclespawner part_reference)
    {
        this.part_reference = part_reference;
    }
}
