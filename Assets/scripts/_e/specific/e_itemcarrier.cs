using UnityEngine;

// an entity, like a crate, that has an inventory

public class e_itemcarrier : MonoBehaviour
{
    public e_genericentity e;

    public ItemStack[] items;

    void Awake()
    {
        e = GetComponent<e_genericentity>();
        
    }

    


    public void UpdateEntityData()
    {
        string str = ItemStack.ParseArrayToString(items);

        e.data.SetDataEntry("inventory", str);
    }
    public void UpdateFromData()
    {
        string data = e.data.GetDataEntry("inventory");

        items = ItemStack.ParseArrayFromString(data);
    }
}
