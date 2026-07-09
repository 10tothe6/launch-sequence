using UnityEngine;

public class test_drawmultipleinventories : MonoBehaviour
{
    private ui_inventories comp;

    public crft_inventory inventoryPart;
    private bool isBuilt;

    void Awake()
    {
        comp = GetComponent<ui_inventories>();
    }

    void Update()
    {
        // actual script logic

        if (!isBuilt)
        {
            comp.BuildMenus(new System.Func<inv_inventorydata>[] {() => {return inventoryPart.data;}});
            isBuilt = true;
        }
    }
}
