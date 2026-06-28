using UnityEngine;
using UnityEngine.InputSystem;

// all the 'ctrl+' keyboard shortcuts in one place

public class cmd_shortcuts : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.ctrlKey.isPressed)
        {
            
            // ctrl+i is toggle map icons
            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                WorldManager.Instance.ToggleMapIcons();
            }


            // ctrl+f is toggle flight
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                // TODO: this
            }
        } 

        // ***
        // temp mic controls
        // ***

        // ctrl + m is mute/unmute
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            if (Input.micStatus == (ushort)audio_micstatus.Muted)
            {
                Input.micStatus = (ushort)audio_micstatus.None;
            } else
            {
                Input.micStatus = (ushort)audio_micstatus.Muted;
            }
        }

        // ctrl + n is deafen/undeafen
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            if (Input.micStatus == (ushort)audio_micstatus.Deafened)
            {
                Input.micStatus = (ushort)audio_micstatus.None;
            } else
            {
                Input.micStatus = (ushort)audio_micstatus.Deafened;
            }
        }
    }
}
