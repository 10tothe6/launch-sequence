using UnityEngine;

public class ui_resourcecompartementwidget : MonoBehaviour
{
    public Transform t_resourceWidgetContainer;
    public GameObject p_resourceWidget;

    public RectTransform rt_bg;

    public void BuildWidget(crft_resourcecompartement data)
    {
        float totaLWidth = rt_bg.sizeDelta.x;
        float widthSum = 0;

        for (int i = 0; i < data.contained_resources.Count; i++)
        {
            GameObject g_new = Instantiate(p_resourceWidget, t_resourceWidgetContainer);

            g_new.transform.localPosition = new Vector3(widthSum, 0, 0);

            g_new.GetComponent<ui_resourcewidget>().BuildWidget(data.contained_resources[i], data.max_capacity, totaLWidth);

            widthSum += totaLWidth * (data.contained_resources[i].resource_amount / data.max_capacity);
        }
    }
}
