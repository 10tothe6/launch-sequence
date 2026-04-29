using UnityEngine;

public class test_addtoinventory : MonoBehaviour
{
    void Start()
    {
        GetComponent<Inventory>().AddItem(0,1);
    }
}
