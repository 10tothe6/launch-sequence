using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ui_canisterwidget : MonoBehaviour
{
    public List<int> showingTooltip;

    public Transform t_compartementWidgetContainer;
    public GameObject p_compartementWidget;
    public float spaceBetweenWidgets;


    public Transform t_cursorStuff;
    public TextMeshProUGUI tx_resourceName;
    public TextMeshProUGUI tx_resourceAmt;


    public TextMeshProUGUI tx_partName;

    public void BuildWidget(string part_name, List<crft_resourcecompartement> compartements)
    {
        tx_partName.text = part_name;

        for (int i = 0; i < compartements.Count; i++)
        {
            GameObject g_new = Instantiate(p_compartementWidget, t_compartementWidgetContainer);

            // make sure to set the local position of the widgets so they don't overlap
            g_new.transform.localPosition = new Vector3(0, i * -(g_new.GetComponent<ui_instantiatable>().effectiveHeight + spaceBetweenWidgets));

            ui_resourcecompartementwidget comp = g_new.GetComponent<ui_resourcecompartementwidget>();

            comp.BuildWidget(compartements[i]);
        }
    }

    void Update()
    {
        t_cursorStuff.position = Input.mousePosition;

        t_cursorStuff.gameObject.SetActive(showingTooltip.Count > 0);
    }
}
