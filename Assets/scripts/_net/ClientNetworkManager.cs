using UnityEngine;
using Riptide;
using Riptide.Utils;
using System;

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
    remove_player_request = 00001, // banning someone

    chat_message_send = 00100, // probably the only message in category 01

    movement_keypresses = 00400, // trying to move, basically
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

    // pretty sure the client doesn't need to know the actual IDs of the other clients
    public string[] otherUsernames; // the usernames of the other connected clients

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

    public void ConnectToLocalServer() { ConnectToServer("127.0.0.1", 7770);}
    public void ConnectToServer(string ip, ushort port)
    {
        if (username.Length < 1) {cmd.Log("Username has not been set! Cannot join server."); return;}
        
        cmd.Log("Connecting client to server ...");
        client.Connect($"{ip}:{port}");

        isClientActive = true;
    }
    private void DidConnect(object sender, EventArgs e)
    {
        // send basic info to server
        cmd.Log("Connected to server.");
        SendJoinRequestToServer();
    }
    private void FailedToConnect(object sender, EventArgs e)
    {
        // back to the menu
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

    public void SendJoinRequestToServer()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.join_request);
        message.AddString(username);
        message.AddString(Program.Instance.version);
        Instance.client.Send(message);
    }

    public void SendChatMessageToServer(string msg)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.chat_message_send);
        // TODO: add client index
        message.AddString(msg);
        // TODO: timestamp?
    }

    public void RequestKickPlayer(string username)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.remove_player_request);
        
        message.AddString(username);
        // @
    }

    [MessageHandler((ushort)ServerToClientId.join_denied)]
    private static void HandleJoinDenial(Message message)
    {
        Debug.Log("Server kicked us out.");
        Instance.client.Disconnect();
    }

    [MessageHandler((ushort)ServerToClientId.join_permitted)]
    private static void HandleJoinConfirmation(Message message)
    {
        string[] otherPlayerNames = message.GetStrings();
        Instance.otherUsernames = otherPlayerNames;

        // once we've confirmed that we're in the server,
        // open up the sim setup menu

        // TODO: switch to lobby
    }
}
