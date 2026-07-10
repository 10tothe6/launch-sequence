using UnityEngine;

public class test_fillwithitems : MonoBehaviour
{
    void Start()
    {
        GetComponent<crft_inventory>().data.AddItem(new inv_itemstack(0, 2, 0));
        GetComponent<crft_inventory>().data.AddItem(new inv_itemstack(2, 4, 1));

        GetComponent<crft_inventory>().data.AddItem(new inv_itemstack(1, 3, 6));
    }
}
