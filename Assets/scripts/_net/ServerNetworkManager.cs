using UnityEngine;
using Riptide;
using Riptide.Utils;
using System.Collections.Generic;
using UnityEngine.Events;

// (not bothering with a _net prefix here, cuz its a HL script)

// SERVER!

public enum ServerToClientId : ushort
{
    join_permitted = 10000, // tell a client that they can join (also send them the player list)
    join_denied = 10001, // tell a client that they can't join (usually bc their username is taken or their version is wrong)
    player_connected = 10002, // new player joined (tell to all clients)
    player_disconnected = 10003, // new player quit OR WAS BANNED (tell to all clients))

    chat_message_update = 10100, // tell to all clients

    player_positions = 10400, // an entity positioning update
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
    public string serverIP;
    public ushort serverPort;

    public bool isServerActive;

    // trying to keep this central and easy-to-access, unlike the old version of drivetrain
    // come to think of it the weird Player class caused a lot of issues
    // NOT ORDERED AT ALL, KEEP IN MIND
    public List<net_connectedclient> connectedClients; 

    public bool useWhitelist;
    // ips of whitelisted users
    // ips of blacklisted users
    public List<string> whitelist;
    public List<string> blacklist;

    // UNITY EVENTS *************
    public UnityEvent onJoinServer;
    public UnityEvent<string> onPlayerJoin;
    public UnityEvent<string> onPlayerLeave;
    // ************

    public static string[] GetConnectedUsernames()
    {
        return new string[] {"localplayer"};
    }

    public void WhitelistPlayer(string username)
    {
        
    }
    public void BlacklistPlayer(string username)
    {
        
    }

    public void KickPlayer(string username)
    {
        
    }

    public void BanPlayer(string username)
    {
        BlacklistPlayer(username);
        KickPlayer(username);
    }

    private void Start()
    {
        connectedClients = new List<net_connectedclient>();
        server = new Server();
    }

    public void StartServer(ushort port, ushort maxClientCount)
    {
        cmd.LogRaw("[Server] Starting server ...");
        server.Start(port, maxClientCount);
        isServerActive = true;
    }
    
    // hosting and joining a server
    public void StartAndJoinServer(ushort port, ushort maxClientCount)
    {
        StartServer(port, maxClientCount);
        ClientNetworkManager.Instance.ConnectToLocalServer();
    }

    // starting a singleplayer world
    public void StartSingleplayerServer()
    {
        cmd.LogRaw("[Server] Setting up singleplayer server on port " + 7770 + "...");
        StartServer(7770, 1);
        ClientNetworkManager.Instance.username = "localplayer";
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

        cmd.LogRaw($"[Server] Received join request from '{username}'. Validating...");        

        if (Instance.IsUsernameTaken(username))
        {
            Instance.SendJoinDenial(fromClientId, "duplicate username");
            cmd.LogRaw($"[Server] Client denied. Reason: duplicate username.");
        } 
        else if (Program.Instance.version != version)
        {
            Instance.SendJoinDenial(fromClientId, "wrong version");
            cmd.LogRaw($"[Server] Client denied. Reason: wrong game version.");
        }

        
        else
        {
            bool passedListCheck = false;

            if (Instance.useWhitelist)
            {
                // whitelist check
                if (Instance.whitelist.Contains(util_network.RemovePortFromIp(Instance.server.Clients[fromClientId-1].ToString())))
                {
                    // on the whitelist, so we good
                    passedListCheck = true;
                } else
                {
                    Instance.SendJoinDenial(fromClientId, "not whitelisted");
                    cmd.LogRaw($"[Server] Client denied. Reason: not on whitelist.");
                }
            } else
            {
                // blacklist check
                if (!Instance.blacklist.Contains(util_network.RemovePortFromIp(Instance.server.Clients[fromClientId-1].ToString())))
                {
                    // not on blacklist, so we good
                    passedListCheck = true;
                } else
                {
                    Instance.SendJoinDenial(fromClientId, "blacklisted");
                    cmd.LogRaw($"[Server] Client with denied. Reason: on blacklist.");
                }
            }

            if (passedListCheck)
            {
                cmd.LogRaw($"[Server] Client accepted.");

                Instance.connectedClients.Add(new net_connectedclient(username, fromClientId));

                Instance.SendJoinConfirm(fromClientId);
            }
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

    public void SendJoinDenial(ushort toClientId, string reason)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.join_denied);
        message.AddString(reason);
        Instance.server.Send(message, toClientId);

        // nobody else really needs to see that someone tried to join
    }

    public void SendJoinConfirm(ushort toClientId)
    {
        cmd.LogRaw($"[Server] Sending player list to new client...");

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.join_permitted);

        // converting the connected client list to a string, then shipping it over
        string[] connectedClients = new string[Instance.connectedClients.Count];

        for (int i = 0; i < connectedClients.Length; i++)
        {
            connectedClients[i] = Instance.connectedClients[i].ParseToString();
        }

        message.AddStrings(connectedClients);
       
        Instance.server.Send(message, toClientId);

        // so the one client has been confirmed, great
        // now we tell everyone else that there's a new player
        Message toOthers = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.player_connected);

        // the username and permission level (net_connectedclient) get sent over
        toOthers.AddString(GetClient(toClientId).ParseToString());
        int otherPlayerCount = Instance.connectedClients.Count - 1;
        cmd.LogRaw($"[Server] Sending player join notification to {otherPlayerCount} other clients...");
        SendToAllExcept(toClientId, toOthers);
    }

    public static net_connectedclient GetClient(ushort id)
    {
        for (int i = 0; i < Instance.connectedClients.Count; i++)
        {
            if (Instance.connectedClients[i].client_index == id)
            {
                return Instance.connectedClients[i];
            }
        }
        return null;
    }

    public static void SendToAllExcept(ushort except, Message msg)
    {
        for (int i = 0; i < Instance.connectedClients.Count; i++)
        {
            if (i == except) {continue;}
            Instance.server.Send(msg, (ushort)i);
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
