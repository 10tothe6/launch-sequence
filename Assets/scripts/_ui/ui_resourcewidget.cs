using UnityEngine.UI;
using UnityEngine;
using NUnit.Framework;

public class ui_resourcewidget : MonoBehaviour
{
    public RectTransform rt_bg;

    public float width_required_for_icon;

    public Image i_icon;

    private mtrl_containedresource data;

    private int hash;

    public void BuildWidget(mtrl_containedresource data, float capacity, float totalWidth)
    {
        this.data = data;



        rt_bg.sizeDelta = new Vector2(totalWidth * (data.resource_amount / capacity), rt_bg.sizeDelta.y);

        rt_bg.GetComponent<Image>().color = data.GetData().color;

        if (rt_bg.sizeDelta.x > width_required_for_icon)
        {
            i_icon.gameObject.SetActive(true);

            i_icon.transform.localPosition = new Vector3(rt_bg.sizeDelta.x / 2 - 17.5f, 0);
            i_icon.sprite = data.GetData().icon;
        } else
        {
            i_icon.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (ui_canvasutils.IsCursorInteract(rt_bg.gameObject, true))
        {
            if (hash == 0)
            {
                hash = Random.Range(1, int.MaxValue);
                UIManager.Instance.canister.showingTooltip.Add(hash);
            }

            // show the information of the resource
            UIManager.Instance.canister.tx_resourceName.text = data.resource_name;
            UIManager.Instance.canister.tx_resourceAmt.text = data.resource_amount.ToString() + " m^3";
        } else
        {
            if (hash != 0)
            {
                UIManager.Instance.canister.showingTooltip.Remove(hash);
            }

            hash = 0;
        }
    }
}
