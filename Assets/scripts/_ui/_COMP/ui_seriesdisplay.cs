using UnityEngine.UI;
using UnityEngine;

// displaying various icons depending on how big a number is

public class ui_seriesdisplay : MonoBehaviour
{
    [Header("pick one of these two")]
    public Image iComp;
    public bool useSpriteRenderer; // if you want to not use an image component
    public SpriteRenderer sComp;

    [Space(20)]
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
                if (useSpriteRenderer)
                {
                    sComp.sprite = icons[i];
                } else
                {
                    iComp.sprite = icons[i];
                }
            }
        }
    }
}
