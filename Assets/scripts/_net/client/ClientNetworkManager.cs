using UnityEngine;
using Riptide;
using Riptide.Utils;
using System;
using System.Linq;

// (not bothering with a _net prefix here, cuz its a HL script)

// CLIENT!

// ids use a classification system:
// first digit is either 0 (client->server) or 1 (server-client)

// next 2 digits are the category:

// 00 is basic network info (connection, disconnection)
// 01 is chat messages
// 02 is server state updates (e.g. entering a match)
// 03 is level data, like a new robot or level
// 04 is transform updating, new positions

// last 2 digits is an index
public enum ClientToServerId : ushort
{
    join_request = 00000, // can i join this server?
    leave_request = 00001, // 'fuck this shit I'm out'

    chat_message_send = 00100,
    command_request = 00101, // same message for any command, for simplicity
    // ^ this includes kicking, banning, and so on
}

public class ClientNetworkManager : MonoBehaviour
{
    private static ClientNetworkManager _instance;

    public static ClientNetworkManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("Duplicate NetworkManager instance in scene!");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public string username; // the username associated with the current client

    public bool isClientActive;
    public Client client {get; private set;}

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        client = new Client();

        client.Connected += DidConnect;
        client.ConnectionFailed += FailedToConnect;
        client.Disconnected += DidDisconnect;
    }

    public void ConnectToLocalServer() { ConnectToServer("127.0.0.1", NetworkResources.defaultServerPort);}
    public void ConnectToServer(string ip, ushort port)
    {
        if (username.Length < 1) {
            cmd.LogRaw("[Client] Username has not been set! Cannot join server.", Color.yellow); 
            return;
        }
        
        cmd.LogRaw("[Client] Connecting to local server ...", Color.yellow);
        ServerNetworkManager.Instance.serverIP = ip;
        ServerNetworkManager.Instance.serverPort = port;
        client.Connect($"{ip}:{port}");

        isClientActive = true;
    }
    private void DidConnect(object sender, EventArgs e)
    {
        // send basic info to server
        cmd.LogRaw("[Client] Found server at ip: " + ServerNetworkManager.Instance.serverIP + ". Sending handshake...", Color.yellow);
        ClientSenders.Instance.SendJoinRequestToServer();
    }
    private void FailedToConnect(object sender, EventArgs e)
    {
        // back to the menu
        UIManager.Instance.SwitchMenu("join server menu");
        ui_infoalerts.Instance.ShowFullscreenAlert("connection failed!",Color.orange);

        isClientActive = false;
    }
    private void DidDisconnect(object sender, EventArgs e)
    {
        // back to the menu
        isClientActive = false;
    }

    private void FixedUpdate()
    {
        if (isClientActive) client.Update();
    }

    private void OnApplicationQuit()
    {
        if (isClientActive) client.Disconnect();
    }
}
