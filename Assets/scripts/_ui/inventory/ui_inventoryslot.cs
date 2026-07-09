using UnityEngine.UI;
using UnityEngine;

public class ui_inventoryslot : MonoBehaviour
{
    private inv_itemstack cachedItemData;
    private RectTransform rt;

    private Image i_itemIcon;
    private RectTransform rt_itemIcon;
    public GameObject p_itemIcon;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Initialize(Transform t_itemIconContainer)
    {
        GameObject g_itemIcon = Instantiate(p_itemIcon, t_itemIconContainer);

        g_itemIcon.transform.position = transform.position;

        i_itemIcon = g_itemIcon.GetComponent<Image>();
        rt_itemIcon = g_itemIcon.GetComponent<RectTransform>();

        g_itemIcon.SetActive(false); // no item
    }

    public void SetItem(inv_itemstack data)
    {
        cachedItemData = data;

        // first, the dimensions
        rt_itemIcon.sizeDelta = new Vector2(rt.sizeDelta.x * data.extendHorizontal, rt.sizeDelta.y * data.extendVertical);
        i_itemIcon.gameObject.SetActive(true);
        i_itemIcon.color = Color.white;
    }
}
