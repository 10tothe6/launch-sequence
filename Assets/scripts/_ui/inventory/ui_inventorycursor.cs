using UnityEngine;
using UnityEngine.UI;

public class ui_inventorycursor : MonoBehaviour
{
    [HideInInspector]
    public inv_itemstack heldItem;

    public RectTransform rt_itemIcon;
    public Image i_itemIcon;

    public float inventoryCellSize;

    void Awake()
    {
        heldItem = null;
    }

    public void GrabItem(inv_itemstack item)
    {
        heldItem = item;
        DrawHeldItem();
    }

    private void DrawHeldItem()
    {
        if (heldItem == null)
        {
            // just show nothing
        } else
        {
            // first, the dimensions
            rt_itemIcon.sizeDelta = new Vector2(inventoryCellSize * heldItem.extendHorizontal, inventoryCellSize * heldItem.extendVertical);
            i_itemIcon.gameObject.SetActive(true);
            i_itemIcon.color = Color.white;

            // no button on the cursor
        }
    }
}
