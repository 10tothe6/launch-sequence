using UnityEngine;

// the ONLY purpose of this class is to take the mic volume and pass it to a series display
// that's it

// not used rn, feel free to decide if it is to be deleted

public class ui_localmicvolume : MonoBehaviour
{
    public ui_seriesdisplay display;

    void Update()
    {
        display.SetValue(Input.micPeakValue);
    }
}
