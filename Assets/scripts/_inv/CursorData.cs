using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

//this class is for cursor stuff
public class CursorData : MonoBehaviour
{
    public ItemStack heldItem;

    void Awake()
    {
        heldItem = null;
    }

    void Update()
    {
        //toggle the sprite for the held item based on whether there is one
        if (heldItem != null)
        {
            transform.GetChild(0).gameObject.GetComponent<Image>().sprite = WorldData.Instance.items[heldItem.id].icon;
                transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        transform.GetChild(0).position = Input.mousePosition;
    }

    public void GiveItem(ItemStack itemToGive) {
        heldItem = itemToGive;
    }

    public void Clear() {
        heldItem = null;
    }
}
