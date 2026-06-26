using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class ui_hostservermenu : MonoBehaviour
{
    public TMP_InputField in_maxPlayerCount;
    public TMP_InputField in_port;
    public TMP_InputField in_botCount;

    public TMP_InputField in_serverName;

    public ui_selector gameModeSelector;

    void Awake()
    {
        ApplyDefaultValues();
    }

    // TODO: put all of these through network resources
    public void ApplyDefaultValues()
    {
        in_maxPlayerCount.text = "2";
        in_port.text = NetworkResources.defaultServerPort.ToString();
        in_botCount.text = "4";
        in_serverName.text = "max's server";

        gameModeSelector.SetValue(1);
    }


    // TODO: agency flag and colors
    // returns null if data bad
    public net_serverhostingdata AssembleHostingData()
    {
        net_serverhostingdata data = new net_serverhostingdata();
        
        if (string.IsNullOrEmpty(in_serverName.text)) {
            ui_infoalerts.Instance.ShowFullscreenAlert("please enter a server name!",Color.red);
            return null;}
        if (!util_string.CheckForIlligelCharacters(in_serverName.text))
        {
            ui_infoalerts.Instance.ShowFullscreenAlert("please do not use illegal characters in your server name!",Color.red);
            return null;}
        
        data.server_name = in_serverName.text;
        data.server_description = "A multiplayer server.";


        // really these numbers are irrelevant
        ushort parsedPlayerCount = 1;
        ushort parsedPort = 7700;
        
        if (ushort.TryParse(in_maxPlayerCount.text, out parsedPlayerCount)) {
            data.max_player_count = parsedPlayerCount;
        } else {ui_infoalerts.Instance.ShowFullscreenAlert("please enter a max client count!",Color.red); return null;}
        if (ushort.TryParse(in_port.text, out parsedPort))
        {
            data.server_port = parsedPort;
        } else {ui_infoalerts.Instance.ShowFullscreenAlert("please enter a port for the server!",Color.red); return null;}

        data.gamemode = gameModeSelector.currentIndex;

        // only relevant for hardcore
        if (data.gamemode == 2)
        {
            ushort parsedBotCount = 0;
            
            if (ushort.TryParse(in_botCount.text, out parsedBotCount))
            {
                data.bot_count = parsedBotCount;
            } else
            {
                ui_infoalerts.Instance.ShowFullscreenAlert("please enter a bot count!",Color.red);
                return null;
            }
        }
        else
        {
            data.bot_count = 0;
        }


        // unused rn
        // data.flag = NetworkResources.defaultServerHostingData.flag;
        // data.col1 = NetworkResources.defaultServerHostingData.col1;

        return data;
    }


    public void AttemptServerHost()
    {
        net_serverhostingdata hostingData = AssembleHostingData();

        if (hostingData != null) // no null means data good
        {
            GameManager.gameState = GameState.InGame;

            // port is actually not considered rn
            ServerNetworkManager.Instance.StartMultiplayerServer(hostingData);
        } else
        {
            // the alert should have already been called by the above func
        }
    }
}
