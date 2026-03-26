using System;
using UnityEngine;

public class ui_worldspaceelement : MonoBehaviour
{
    public Transform t_element;

    public Func<Vector3> positionSource;

    public bool isHidden;

    public float distanceLimit;

    public Func<bool> additionalDrawCriteria;

    public void Show()
    {
        isHidden = false;
    }

    public void Hide()
    {
        isHidden = true;
    }

    void Update()
    {
        if (positionSource != null)
        {
            Vector3 wPos = positionSource.Invoke();

            t_element.position = Camera.main.WorldToScreenPoint(wPos);

            if (isHidden)
            {
                t_element.gameObject.SetActive(false);
            } else
            {
                t_element.gameObject.SetActive(
                    Vector3.Angle(CameraController.t_cam.forward, wPos - CameraController.t_cam.position) < 90
                    && Vector3.Distance(wPos, CameraController.t_cam.position) < distanceLimit && 
                    additionalDrawCriteria.Invoke());
            }
        }
    }
}
