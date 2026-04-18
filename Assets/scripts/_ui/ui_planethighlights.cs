using UnityEngine;

public class ui_planethighlights : MonoBehaviour
{
    public GameObject p_icon;
    public Transform t_iconContainer;

    void Start()
    {
        WorldManager.Instance.onNewWorldGenerate.AddListener(Setup);
    }

    void Setup()
    {
        for (int i = 0; i < cb_solarsystem.Instance.monoBodies.Count; i++)
        {
            Transform t_newIcon = Instantiate(p_icon, t_iconContainer).transform;

            ui_worldspaceelement comp = t_newIcon.GetComponent<ui_worldspaceelement>();
            comp.distanceLimit = Mathf.Infinity;

            int j = i;

            comp.additionalDrawCriteria = () =>
            {
                return !UIManager.Instance.isInMapView && !LocalPlayer.IsInSandbox() && cb_solarsystem.Instance.monoBodies[j].ShouldIconBeVisible();
            };

            comp.GetComponent<ui_planetpoint>().Setup(cb_solarsystem.Instance.monoBodies[i].gameObject.name);
            
            comp.positionSource = () => cb_solarsystem.Instance.monoBodies[j].transform.position;
        }
    }
}
