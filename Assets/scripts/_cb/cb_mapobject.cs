using UnityEngine;

public class cb_mapobject : MonoBehaviour
{
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
    }

    public void UpdatePosition()
    {
        
    }
}
