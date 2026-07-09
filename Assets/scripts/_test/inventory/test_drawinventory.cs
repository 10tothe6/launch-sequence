using UnityEngine;

public class test_drawinventory : MonoBehaviour
{
    private ui_inventorywidget comp;

    public crft_inventory inventoryPart;

    void Awake()
    {
        comp = GetComponent<ui_inventorywidget>();
    }

    void Start()
    {
        // actual script logic

        comp.BuildMenu(() => {return inventoryPart.data;});
    }
}
