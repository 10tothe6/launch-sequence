using UnityEngine;

public class test_fillwithitems : MonoBehaviour
{

    public inv_itemstack[] items;

    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            GetComponent<crft_inventory>().data.AddItem(items[i]);
        }
    }
}
