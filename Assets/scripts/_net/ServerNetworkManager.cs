using UnityEngine;
using Riptide;
using Riptide.Utils;
using System.Collections.Generic;

// (not bothering with a _net prefix here, cuz its a HL script)

// SERVER!

public enum ServerToClientId : ushort
{
    join_permitted = 10000, // tell a client that they can join (also send them the player list)
    join_denied = 10001, // tell a client that they can't join (usually bc their username is taken or their version is wrong)
    player_connected = 10002, // new player joined
    player_disconnected = 10003, // new player quit

    remove_player = 10004, // for whatever reason

    chat_message_update = 10100,

    robot_select = 10300, // someone updated their robot
    level_select = 10301, // someone selected a level

    player_positions = 10400,
    object_positions = 10401,
}

public class ServerNetworkManager : MonoBehaviour
{
    private static ServerNetworkManager _instance;

    public static ServerNetworkManager Instance
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

    public Server server {get; private set;}

    public bool isServerActive;

    // trying to keep this central and easy-to-access, unlike the old version of drivetrain
    // come to think of it the weird Player class caused a lot of issues
    public List<net_connectedclient> connectedClients;

    private void Start()
    {
        connectedClients = new List<net_connectedclient>();
        server = new Server();
    }

    public void StartServer(ushort port, ushort maxClientCount)
    {
        Debug.Log("Starting Drivetrain server v0.1.0 ...");
        server.Start(port, maxClientCount);
        isServerActive = true;
    }

    public void StartAndJoinServer(ushort port, ushort maxClientCount)
    {
        StartServer(port, maxClientCount);
        ClientNetworkManager.Instance.ConnectToLocalServer();
    }

    public void StartSingleplayerServer()
    {
        StartServer(7770, 1);
        ClientNetworkManager.Instance.ConnectToLocalServer();
    }

    private void FixedUpdate()
    {
        if (isServerActive) server.Update();
    }

    private void OnApplicationQuit()
    {
        if (isServerActive)
        {
            StopServer();
        } 
    }

    public void StopServer()
    {
        isServerActive = false;
        server.Stop();
    }

    [MessageHandler((ushort)ClientToServerId.join_request)]
    private static void HandleJoinRequest(ushort fromClientId, Message message)
    {
        string username = message.GetString();
        string version = message.GetString();

        if (Instance.IsUsernameTaken(username) || Program.Instance.version != version)
        {
            Instance.SendJoinDenial(fromClientId);
            Debug.Log($"Client {fromClientId} with username {username} was denied entry.");
        } else
        {
            Instance.SendJoinConfirm(fromClientId);
            // add the client AFTER we've sent over the player list to avoid having the client think itself is another player
            Instance.connectedClients.Add(new net_connectedclient(username, fromClientId));

            Debug.Log($"Client {fromClientId} with username {username} was permitted to join.");
        }
    }

    [MessageHandler((ushort)ClientToServerId.chat_message_send)]
    private static void HandleChatMessage(ushort fromClientId, Message message)
    {
        string msg = message.GetString();
        Instance.ProcessChatMessage(fromClientId, msg);
    }

    public void ProcessChatMessage(ushort fromClientId, string msg)
    {
        for (int i = 0; i < connectedClients.Count; i++)
        {
            // this is my way of sending a message to everyone except for one person
            // TODO: make this its own func
            if (connectedClients[i].client_index != fromClientId) // no need to send back to the sender
            {
                SendChatMessage(connectedClients[i].client_index, GetUsernameFromIndex(fromClientId), msg);
            }
        }
    }

    // can't pass the index to the client bc they don't know what that is
    public void SendChatMessage(ushort toClientId, string fromUser, string msg)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.chat_message_update);
        message.AddString(fromUser);
        message.AddString(msg);
        Instance.server.Send(message, toClientId);
    }

    public void SendJoinDenial(ushort toClientId)
    {
        string reasoning = "Join request denied.";

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.join_denied);
        message.AddString(reasoning);
        Instance.server.Send(message, toClientId);

        // nobody else really needs to see that someone tried to join
    }

    public void SendJoinConfirm(ushort toClientId)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.join_permitted);
        message.AddStrings(GetClientUsernames());
        Instance.server.Send(message, toClientId);

        // we also need to tell the other players there's a new kid in town
        for (int i = 0; i < connectedClients.Count; i++)
        {
            
        }
    }

    public string GetUsernameFromIndex(ushort index)
    {
        for (int i = 0; i < connectedClients.Count; i++)
        {
            if (connectedClients[i].client_index == index)
            {
                return connectedClients[i].username;
            }
        }

        return "";
    }
    
    public string[] GetClientUsernames()
    {
        string[] result = new string[connectedClients.Count];

        for (int i = 0; i < connectedClients.Count; i++)
        {
            result[i] = connectedClients[i].username;
        }

        return result;
    }

    public bool IsUsernameTaken(string username)
    {
        for (int i = 0; i < connectedClients.Count; i++)
        {
            if (connectedClients[i].username == username)
            {
                return true;
            }
        }
        return false;
    }
}
