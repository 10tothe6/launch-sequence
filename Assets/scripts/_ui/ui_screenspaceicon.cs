using UnityEngine.UI;
using UnityEngine;

// drawing a gameobject to the screen

public class ui_screenspaceicon : MonoBehaviour
{
    public Sprite icon;
    public GameObject p_icon;

    public Transform t_icon;
    public Transform t_uiContainer;

    public void Initialize()
    {
        t_icon = Instantiate(p_icon, t_uiContainer).transform;
        t_icon.GetComponent<Image>().sprite = icon;
    }

    void OnDestroy()
    {
        // TODO: cleanup stuff
    }

    // might change this to an update func that I can control later
    void Update()
    {
        if (Vector3.Angle(Camera.main.transform.forward, transform.position - Camera.main.transform.position) > 90)
        {
            t_icon.gameObject.SetActive(false);
        } else
        {
            t_icon.gameObject.SetActive(true);
        }
        t_icon.position = Camera.main.WorldToScreenPoint(transform.position);
    }
}
