using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpacityController : MonoBehaviour
{
    public TextMeshProUGUI textComp;

    private float opacityTarget;
    public float lerpFactor;

    void Update()
    {
        textComp.color = new Color(textComp.color.r, textComp.color.g, textComp.color.b, Mathf.Lerp(textComp.color.a, opacityTarget, lerpFactor));
    }

    public void Flash()
    {
        opacityTarget = 0;
        textComp.color = new Color(textComp.color.r, textComp.color.g, textComp.color.b, 1);
    }
}
