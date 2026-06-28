using UnityEngine;

public class ui_localmicvolume : MonoBehaviour
{
    public ui_seriesdisplay display;

    void Update()
    {
        display.SetValue(Input.micPeakValue);
    }
}
