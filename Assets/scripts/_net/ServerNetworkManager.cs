using UnityEngine;
using Riptide;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

// (not bothering with a _net prefix here, cuz its a HL script)

// SERVER!

public enum ServerToClientId : ushort
{
    join_permitted = 10000, // tell a client that they can join (also send them the player list)
    join_denied = 10001, // tell a client that they can't join (usually bc their username is taken or their version is wrong)
    player_connected = 10002, // new player joined (tell to all clients)
    player_disconnected = 10003, // new player quit OR WAS BANNED (tell to all clients))

    chat_message_update = 10100, // tell to all clients

    spawn_entity = 10200,
    kill_entity = 10201,
    entity_control = 10202, // does this really need to be its own message?
    entity_position_updates = 10203, // updates, plural (uses arrays)
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

    public UnityEvent<net_connectedclient> onClientConnect; // all the client data, instead of the username
    // ************

    public static string[] GetConnectedUsernames()
    {
        return new string[] {"localplayer"};
    }

    public void ChangeClientPermissions(string username, ushort newPermissionLevel)
    {
        if (Instance.isServerActive)
        {
            GetClientFromUsername(username).permissionLevel = newPermissionLevel;
        } else
        {
            ClientNetworkManager.Instance.SendCommandRequest(cmd_console.GetCommandData("p"), new string[] {username, newPermissionLevel.ToString()});
        }
    }

    public void WhitelistPlayer(string username)
    {
        if (Instance.isServerActive)
        {
            // easy if we're the local client

            // TODO: use the GetIP() function
            whitelist.Add(util_network.RemovePortFromIp(Instance.server.Clients[GetClientFromUsername(username).client_index-1].ToString()));
        } else
        {
            // sending a 'command request' over to the server
            ClientNetworkManager.Instance.SendCommandRequest(cmd_console.GetCommandData("wlist"), new string[] {username});
        }
    }
    public void BlacklistPlayer(string username)
    {
        if (Instance.isServerActive)
        {
            // easy if we're the local client

            // TODO: use the GetIP() function
            blacklist.Add(util_network.RemovePortFromIp(Instance.server.Clients[GetClientFromUsername(username).client_index-1].ToString()));
        } else
        {
            // this is harder because we have to deal with permissions
            if (LocalPlayer.localClient.CanUseCommands())
            {
                // sending a 'command request' over to the server
                ClientNetworkManager.Instance.SendCommandRequest(cmd_console.GetCommandData("blist"), new string[] {username});
            }
        }
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
        cmd.LogRaw("[Server] Starting server ...", Color.cyan);
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
        cmd.LogRaw("[Server] Setting up singleplayer server on port " + 7770 + "...", Color.cyan);
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

        cmd.LogRaw($"[Server] Received join request from '{username}'. Validating...", Color.cyan);        

        if (Instance.IsUsernameTaken(username))
        {
            Instance.SendJoinDenial(fromClientId, "duplicate username");
            cmd.LogRaw($"[Server] Client denied. Reason: duplicate username.", Color.cyan);
        } 
        else if (Program.Instance.version != version)
        {
            Instance.SendJoinDenial(fromClientId, "wrong version");
            cmd.LogRaw($"[Server] Client denied. Reason: wrong game version.", Color.cyan);
        }

        
        else
        {
            bool passedListCheck = false;

            if (Instance.useWhitelist)
            {
                // whitelist check

                // TODO: use the GetIP() function
                if (Instance.whitelist.Contains(util_network.RemovePortFromIp(Instance.server.Clients[fromClientId-1].ToString())))
                {
                    // on the whitelist, so we good
                    passedListCheck = true;
                } else
                {
                    Instance.SendJoinDenial(fromClientId, "not whitelisted");
                    cmd.LogRaw($"[Server] Client denied. Reason: not on whitelist.", Color.cyan);
                }
            } else
            {
                // blacklist check

                // TODO: use the GetIP() function
                if (!Instance.blacklist.Contains(util_network.RemovePortFromIp(Instance.server.Clients[fromClientId-1].ToString())))
                {
                    // not on blacklist, so we good
                    passedListCheck = true;
                } else
                {
                    Instance.SendJoinDenial(fromClientId, "blacklisted");
                    cmd.LogRaw($"[Server] Client with denied. Reason: on blacklist.", Color.cyan);
                }
            }

            if (passedListCheck)
            {
                cmd.LogRaw($"[Server] Client accepted.", Color.cyan);

                net_connectedclient newClient = new net_connectedclient(username, fromClientId);

                Instance.connectedClients.Add(newClient);


                // okay yeah this assumes the first client is the local one, but when ISN'T that the case?
                if (LocalPlayer.localClient == null) 
                {
                    LocalPlayer.localClient = newClient;

                    // default permissions
                    LocalPlayer.localClient.permissionLevel = 2;
                } else
                {
                    LocalPlayer.localClient.permissionLevel = 0;
                }

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

    [MessageHandler((ushort)ClientToServerId.command_request)]
    private static void HandleCommandRequest(ushort fromClientId, Message message)
    {
        string cmdName = message.GetString();
        string[] args = message.GetStrings();

        bool arePermissionsValid = false;

        if (cmd_console.GetCommandData(cmdName).needsAdmin)
        {
            arePermissionsValid = GetClient(fromClientId).IsAdmin();
        } else
        {
            arePermissionsValid = GetClient(fromClientId).CanUseCommands();
        }

        if (arePermissionsValid)
        {
            cmd.LogRaw($"[Server] putting through command ({cmdName}) request from client {fromClientId}.", Color.cyan);

            string constructedMessage = "";
            constructedMessage += cmdName;

            for (int i = 0; i < args.Length; i++)
            {
                constructedMessage += " ";
                constructedMessage += args[i];
            }

            // this is a SLIGHT security risk, but who cares its a video game
            // we assume that if we're getting this message then the client has perms (i.e. the check is done client-side)

            // anyways, look at this look at how convinient this is
            cmd_console.Instance.ProcessMessage(constructedMessage);
        } else
        {
            cmd.LogRaw($"[Server] blocked command ({cmdName}) request from client {fromClientId} cuz perms", Color.cyan);
        }
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

    public void SendNewEntity(GameObject g_new)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.spawn_entity);

        // the prefab index is stored in the int, anything else we need is in the string
        e_genericentity comp = g_new.GetComponent<e_genericentity>();
        message.AddString(comp.data.GetRawPackagedData());
        message.AddInt(comp.data.entityPrefabIndex);

        cmd.LogRaw($"[Server] spawning new {g_new.name} entity...", Color.cyan);

        SendToAllExcept(LocalPlayer.localClient.client_index, message);
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
        cmd.LogRaw($"[Server] Sending player list to new client...", Color.cyan);

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.join_permitted);

        // converting the connected client list to a string, then shipping it over
        string[] connectedClients = new string[Instance.connectedClients.Count];
        for (int i = 0; i < connectedClients.Length; i++)
        {
            connectedClients[i] = Instance.connectedClients[i].ParseToString();
        }
        message.AddStrings(connectedClients);

        
        net_packagedentitydata[] entityData = EntityManager.Instance.PackageAllEntityData();

        string[] data1 = net_packagedentitydata.MakeDataArray(entityData);
        int[] data2 = net_packagedentitydata.MakeIndexArray(entityData);

        message.AddStrings(data1);
        message.AddInts(data2);
       
        Instance.server.Send(message, toClientId);

        // so the one client has been confirmed, great
        // now we tell everyone else that there's a new player
        Message toOthers = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.player_connected);

        // the username and permission level (net_connectedclient) get sent over
        toOthers.AddString(GetClient(toClientId).ParseToString());
        int otherPlayerCount = Instance.connectedClients.Count - 1;
        cmd.LogRaw($"[Server] Sending player join notification to {otherPlayerCount} other clients...", Color.cyan);
        SendToAllExcept(toClientId, toOthers);

        // temp temp temp temp temp
        EntityManager.Instance.PutClientInFreecam(toClientId);
        // putting the player in the first planetary system (also temp)
        Instance.SystemTeleport(ServerNetworkManager.GetClient(toClientId).controllingEntity, 0);
    }

    public void SystemTeleport(e_genericentity entity, int index)
    {
        if (entity == null) {cmd.Log("you can't system teleport nothing, dipshit"); return;}
        entity.data.SetPosition(cb_solarsystem.Instance.monoBodies[index + 2].pose.data.GetPosition().Add(Vector3.right * WorldManager.SeaLevelRadius(index + 2) * 2));
    }

    public void SendEntityPositionUpdates(int[] entityIds)
    {
        // commenting this out cuz it fucking fills the entire console
        //cmd.LogRaw($"[Server] sending entity position update for {entityIds.Length} entities...");

        // we obviously don't need to update the server's client
        Message toOthers = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.entity_position_updates);

        toOthers.AddInts(entityIds);

        string[] positionData = new string[entityIds.Length];

        for (int i = 0; i < positionData.Length; i++)
        {
            if (EntityManager.Instance.GetEntityFromIndex(entityIds[i]) == null) {continue;}
            positionData[i] = EntityManager.Instance.GetEntityFromIndex(entityIds[i]).data.localPosition.AsRawString();
        }

        toOthers.AddStrings(positionData);

        SendToAllExceptLocal(toOthers);
    }

    public void SetControllingEntity(ushort clientId, e_genericentity entity)
    {
        net_connectedclient client = ServerNetworkManager.GetClient(clientId);

        client.controllingEntity = entity;

        cmd.LogRaw($"[Server] setting client {clientId} control to {entity.gameObject.name}...", Color.cyan);

        // great, now its done on the server side
        // we still need to update everyone (except the local client, obviously)
        Message toOthers = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.entity_control);

        toOthers.AddInt(clientId);
        toOthers.AddInt(entity.data.entityPrefabIndex);

        SendToAllExceptLocal(toOthers);
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

    public static net_connectedclient GetClientFromUsername(string username)
    {
        for (int i = 0; i < Instance.connectedClients.Count; i++)
        {
            if (Instance.connectedClients[i].username == username)
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
            if (Instance.connectedClients[i].client_index == except) {continue;}
            Instance.server.Send(msg, (ushort)i);
        }
    }

    public static void SendToAllExceptLocal(Message msg)
    {
        int localIndex = -1; // this effectively counts as nothing (no clients have index of -1)
        if (LocalPlayer.localClient != null)
        {
            localIndex = LocalPlayer.localClient.client_index;
        }

        for (int i = 0; i < Instance.connectedClients.Count; i++)
        {
            if (Instance.connectedClients[i].client_index == localIndex) {continue;}
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
