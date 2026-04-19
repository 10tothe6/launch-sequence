using TMPro;
using UnityEngine;

public class ui_hostmenu : MonoBehaviour
{
    public TMP_InputField in_clientCount;
    public TMP_InputField in_port;

    public void AttemptDirectServerConnection()
    {
        string rawPort = "0";
        string rawClientCount = in_clientCount.text;

        ushort port;
        ushort clientCount = 4;

        bool isDataValid = false;
        if (ushort.TryParse(rawPort, out port))
        {
            if (ushort.TryParse(rawClientCount, out clientCount))
            {
                isDataValid = true;
            }
        }

        if (isDataValid)
        {
            GameManager.gameState = GameState.InGame;

            // port is actually not considered rn
            ServerNetworkManager.Instance.StartMultiplayerServer(clientCount);
        } else
        {
            ui_infoalerts.Instance.ShowFullscreenAlert("please enter valid server data!",Color.purple);
        }
    }
}
