using TMPro;
using UnityEngine;

public class ui_connectionmenu : MonoBehaviour
{
    public TMP_InputField in_port;
    public TMP_InputField in_ip;

    public void AttemptServerConnection()
    {
        ui_infoalerts.Instance.ShowFullscreenAlert("Connecting to server...");
    }
}
