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

    key_presses = 00200, // what keys the player is holding

    // 04 is the voice related-category (on both ends)
    voice_packet = 00400,
    mic_status_update = 00401,    
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

    public void ResetAfterDisconnect()
    {
        // first, clear all of the entity data that we had
        EntityManager.Instance.ClearAllEntityData();

        // then, we clear out the solar system in a similar manner
        cb_solarsystem.Instance.ClearAllSolarSystemData();

        // we need to make sure the ServerNetworkManager is reset too
        ServerNetworkManager.Instance.ClearAllServerData();
    }

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
            cmd.LogRaw("[Client] Username has not been set! Cannot join server.", NetworkResources.Instance.clientUpdateColor); 
            return;
        }
        
        cmd.LogRaw("[Client] Connecting to local server ...", NetworkResources.Instance.clientUpdateColor);
        ServerNetworkManager.Instance.serverIP = ip;
        ServerNetworkManager.Instance.serverPort = port;
        client.Connect($"{ip}:{port}");

        isClientActive = true;
    }
    private void DidConnect(object sender, EventArgs e)
    {
        // send basic info to server
        cmd.LogRaw("[Client] Found server at ip: " + ServerNetworkManager.Instance.serverIP + ". Sending handshake...", NetworkResources.Instance.clientUpdateColor);
        ClientSenders.Instance.SendJoinRequestToServer();
    }
    // called if you try to join a server and it can't find the server at all
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

        // make sure we delete all of the objects, resetting for when we join the next server
        // IT IS VITAL WE GET THIS RIGHT
        ResetAfterDisconnect();
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
