using UnityEngine.UI;
using UnityEngine;

public class ui_inventoryslot : MonoBehaviour
{
    private inv_itemstack cachedItemData;
    private RectTransform rt;

    private Image i_itemIcon;
    private RectTransform rt_itemIcon;
    public GameObject p_itemIcon;

    private int cellIndex;
    private ui_inventorywidget parentWidget;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Initialize(Transform t_itemIconContainer, ui_inventorywidget parentWidget, int cellIndex)
    {
        this.parentWidget = parentWidget;
        this.cellIndex = cellIndex;


        GameObject g_itemIcon = Instantiate(p_itemIcon, t_itemIconContainer);

        g_itemIcon.transform.position = transform.position;

        i_itemIcon = g_itemIcon.GetComponent<Image>();
        rt_itemIcon = g_itemIcon.GetComponent<RectTransform>();

        g_itemIcon.SetActive(true); // no item
        i_itemIcon.color = new Color(1,1,1,0f); // transparent, but there, icon
        rt_itemIcon.sizeDelta = new Vector2(rt.sizeDelta.x * 1, rt.sizeDelta.y * 1);
    }

    // THERE IS NO CLEAR ITEM FUNCTION, JUST PASS 'null' IN HERE
    public void SetItem(inv_itemstack data)
    {
        cachedItemData = data;

        // first, the dimensions
        if (data != null)
        {
            rt_itemIcon.sizeDelta = new Vector2(rt.sizeDelta.x * data.extendHorizontal, rt.sizeDelta.y * data.extendVertical);
        } else
        {
            rt_itemIcon.sizeDelta = new Vector2(rt.sizeDelta.x * 1, rt.sizeDelta.y * 1);
        }
        i_itemIcon.gameObject.SetActive(data != null);
        i_itemIcon.color = Color.white;

        // setting what the button does
        if (data != null)
        {
            i_itemIcon.GetComponent<ui_button>().onPress.AddListener(() => HandleInteractionWithSlot());
        }
    }

    public void HandleInteractionWithSlot()
    {
        parentWidget.HandleInteractionAtCell(cellIndex, false);
    }
}
