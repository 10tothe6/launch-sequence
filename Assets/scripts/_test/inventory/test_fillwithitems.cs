using UnityEngine;

public class test_fillwithitems : MonoBehaviour
{
    void Start()
    {
        GetComponent<crft_inventory>().data.AddItem(new inv_itemstack(0, 1, 0));

        GetComponent<crft_inventory>().data.AddItem(new inv_itemstack(0, 1, 6));
    }
}
