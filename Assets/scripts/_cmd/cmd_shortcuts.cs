using UnityEngine;
using UnityEngine.InputSystem;

// all the 'ctrl+' keyboard shortcuts in one place

public class cmd_shortcuts : MonoBehaviour
{
    public cam_freecam freecam;
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
    }
}
