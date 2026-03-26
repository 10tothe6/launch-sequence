using JetBrains.Annotations;
using UnityEngine;

public class cb_mapobject : MonoBehaviour
{
    public Transform t_model;

    public Plotter orbitLine;

    public int selfIndex;
    public int parentIndex;
    public Transform t_parent;

    public void Initialize(Vector3 pos)
    {
        transform.position = pos;

        selfIndex = transform.GetSiblingIndex();
        parentIndex = cb_solarsystem.Instance.monoBodies[selfIndex].data.pConfig.parentIndex;
        if (parentIndex != -1) {t_parent = WorldManager.Instance.t_mapBodyContainer.GetChild(parentIndex);}

        if (parentIndex < 1)
        {
            gameObject.name = selfIndex.ToString();
        } else
        {
            gameObject.name = "m"+ parentIndex.ToString() + "    " + selfIndex.ToString();
        }
        

        // COM and star dont have an orbit
        if (selfIndex > 1)
        {
            // setting up the orbit line
            Vector3[] orbitPositions = cb_solarsystem.Instance.monoBodies[selfIndex].data.pConfig.SampleFullOrbit(
                WorldManager.Instance.GetMapScaleFromFocusedBody(), 50);

            orbitLine.Plot(orbitPositions);

            orbitLine.lr.transform.localPosition = t_parent.position - transform.position;
        }

        // this generates the actual object mesh
        GenerateModel();

        CameraController.Instance.onChangeControlMode.AddListener(ShowHideIcon);
    }

    public void ShowHideIcon()
    {
        if (CameraController.controlMode == (ushort)CameraControlMode.MapView)
        {
            GetComponent<ui_linkedicon>().Show();
        } else {GetComponent<ui_linkedicon>().Hide();}
    }

    // the physical mesh that the body will use
    public void GenerateModel()
    {
        t_model.GetChild(0).localScale = 
        Vector3.one * cb_solarsystem.Instance.monoBodies[selfIndex].data.tConfig.equitorialRadius * 2 * WorldManager.Instance.GetMapScaleFromFocusedBody();
    }

    public void UpdatePosition()
    {
        
    }
}
