using System;
using TMPro;
using UnityEngine;

public class e_mapentity : MonoBehaviour
{
    public ui_worldspaceelement comp;
    public e_generic reference;
    public bool showName;

    public TextMeshProUGUI tx_name;

    public void Initialize()
    {
        if (reference == null) {cmd.Log("problem when creating map entity"); return;}
        tx_name.text = reference.entityName;

        tx_name.gameObject.SetActive(showName);

        comp.distanceLimit = Mathf.Infinity;
        
        comp.positionSource = () => ui_mapview.Instance.ConvertPosition(reference.GetPosition()).ToVector3();
        comp.additionalDrawCriteria = () => UIManager.Instance.isInMapView;
    }
}
