using UnityEngine.UI;
using UnityEngine;

// displaying various icons depending on how big a number is

public class ui_seriesdisplay : MonoBehaviour
{
    public Image iComp;
    public float[] thresholds;
    public Sprite[] icons; // equal in length to 'thresholds'

    public bool updatePeriodic;


    public float value; // do not write directly to this

    public void SetValue(float newValue)
    {
        value = newValue;

        UpdateDisplay();
    }

    void Update()
    {
        if (updatePeriodic)
        {
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (value >= thresholds[i])
            {
                iComp.sprite = icons[i];
            }
        }
    }
}
