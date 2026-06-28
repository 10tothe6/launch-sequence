using UnityEngine.UI;
using UnityEngine;

// originally I was just using a ui_seriesdisplay,
// but now I also have to deal with mute/deafened players

public class ui_micdisplay : MonoBehaviour
{
    public Image iComp;
    public bool useSpriteRenderer;
    public SpriteRenderer sComp;
    public Sprite[] icons;

    public float talkingThreshold; // clearly not in decibels, so idk what this is

    public bool updateFromLocal;

    void Update()
    {
        if (updateFromLocal)
        {
            SetData(Input.micPeakValue, Input.micStatus);
        }
    }

    public void SetData(float micPeakValue, ushort micStatus)
    {
        if (micStatus == (ushort)audio_micstatus.Deafened)
        {
            if (useSpriteRenderer)
            {
                sComp.sprite = icons[3];
            } else {iComp.sprite = icons[3];}
        } else if (micStatus == (ushort)audio_micstatus.Muted)
        {
            if (useSpriteRenderer)
            {
                sComp.sprite = icons[2];
            } else {iComp.sprite = icons[2];}
        } else if (micPeakValue > talkingThreshold)
        {
            if (useSpriteRenderer)
            {
                sComp.sprite = icons[1];
            } else {iComp.sprite = icons[1];}
        } else
        {
            if (useSpriteRenderer)
            {
                sComp.sprite = icons[0];
            } else {iComp.sprite = icons[0];}
        }
    }
}
