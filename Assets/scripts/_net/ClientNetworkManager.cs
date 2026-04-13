using UnityEngine;
using Riptide;
using Riptide.Utils;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

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
    kick_player_request = 00001, // kicking someone

    chat_message_send = 00100,
    command_request = 00101, // same message for any command, for simplicity
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

    public void ConnectToLocalServer() { ConnectToServer("127.0.0.1", 7770);}
    public void ConnectToServer(string ip, ushort port)
    {
        if (username.Length < 1) {cmd.LogRaw("[Client] Username has not been set! Cannot join server."); return;}
        
        cmd.LogRaw("[Client] Connecting to local server ...");
        ServerNetworkManager.Instance.serverIP = ip;
        ServerNetworkManager.Instance.serverPort = port;
        client.Connect($"{ip}:{port}");

        isClientActive = true;
    }
    private void DidConnect(object sender, EventArgs e)
    {
        // send basic info to server
        cmd.LogRaw("[Client] Found server at ip: " + ServerNetworkManager.Instance.serverIP + ". Sending handshake...");
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
        cmd.LogRaw($"[Client] requesting player {username} to be kicked...");

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.kick_player_request);
        
        message.AddString(username); // this is all we need for this one
        
        client.Send(message);
    }

    public void SendCommandRequest(cmd_consolecommand command, string[] args)
    {
        cmd.LogRaw($"[Client] requesting {command.names[0]} command from server...");

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.command_request);

        message.AddString(command.names[0]);
        message.AddStrings(args);

        client.Send(message);
    }
    

    // YOU ARE ENTERING THE HANDLER ZONE vvvv
    // ********************************************************

    [MessageHandler((ushort)ServerToClientId.join_denied)]
    private static void HandleJoinDenial(Message message)
    {
        string reason = message.GetString();

        cmd.LogRaw($"[Client] Join request denied by server. Reason: {reason}");
        Instance.client.Disconnect();
    }

    [MessageHandler((ushort)ServerToClientId.join_permitted)]
    private static void HandleJoinConfirmation(Message message)
    {
        string[] rawClientData = message.GetStrings();
        net_connectedclient[] clientData = net_connectedclient.ParseFromStringArray(rawClientData);

        if (!ServerNetworkManager.Instance.isServerActive)
        {
            cmd.LogRaw($"[Client] Join request accepted. Setting client list ({rawClientData.Length})...");
            ServerNetworkManager.Instance.connectedClients = clientData.ToList();
        } else
        {
            cmd.LogRaw($"[Client] Join request accepted. Client list skipped cuz we're a server");
        }

        string[] rawEntityData1 = message.GetStrings();
        int[] rawEntityData2 = message.GetInts();
        cmd.LogRaw($"[Client] Setting entity list ({rawEntityData1.Length})...");

        for (int i = 0; i < rawEntityData1.Length; i++)
        {
            // first, make the new prefab
            EntityManager.Instance.SpawnNewEntity(rawEntityData2[i], rawEntityData1[i]); 
        }

        LocalPlayer.localClient = ServerNetworkManager.GetClient(Instance.client.Id);

        // default permissions
        if (ServerNetworkManager.Instance.isServerActive)
        {
            LocalPlayer.localClient.permissionLevel = 2;
        } else
        {
            LocalPlayer.localClient.permissionLevel = 0;
        }

        ServerNetworkManager.Instance.onJoinServer.Invoke();
    }

    [MessageHandler((ushort)ServerToClientId.player_connected)]
    private static void HandleNewPlayer(Message message)
    {
        net_connectedclient newPlayer = net_connectedclient.ParseFromString(message.GetString());
        ServerNetworkManager.Instance.onPlayerJoin.Invoke(newPlayer.username);
    }

    [MessageHandler((ushort)ServerToClientId.player_disconnected)]
    private static void HandlePlayerDisconnect(Message message)
    {
        net_connectedclient playerGone = net_connectedclient.ParseFromString(message.GetString());
        ServerNetworkManager.Instance.onPlayerLeave.Invoke(playerGone.username);
    }

    [MessageHandler((ushort)ServerToClientId.entity_control)]
    private static void HandleEntityControlChange(Message message)
    {
        int clientIndex = message.GetInt();
        int entityIndex = message.GetInt();
        // TODO: actually set the control
    }

    [MessageHandler((ushort)ServerToClientId.entity_position_updates)]
    private static void HandleEntityPositionUpdates(Message message)
    {
        int[] entityIndices = message.GetInts();
        string[] entityPositions = message.GetStrings();

        cmd.LogRaw($"[Client] got entity position update for {entityIndices} entities.");

        for (int i = 0; i < entityIndices.Length; i++)
        {
            EntityManager.Instance.GetEntityFromIndex(entityIndices[i]).data.SetPosition(num_precisevector3.FromString(entityPositions[i]));
        }
    }
}
