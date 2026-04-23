using UnityEngine;
using Riptide;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using System.Net;
using UnityEditor;

// (not bothering with a _net prefix here, cuz its a HL script)

// SERVER!

public enum ServerToClientId : ushort
{
    join_permitted = 10000, // tell a client that they can join (also send them the player list)
    join_denied = 10001, // tell a client that they can't join (usually bc their username is taken or their version is wrong)
    player_connected = 10002, // new player joined (tell to all clients)
    player_disconnected = 10003, // new player quit OR WAS BANNED (tell to all clients))
    kick_player = 10004, // force a client to leave
    player_permission_update = 10005,

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

    // update timers for deciding when to send infrequent updates
    // sleeping has no updates
    private float lastLocalizedUpdate;
    private float lastInfluencedUpdate;
    private float lastControlledUpdate;
    // independent is FREQUENT updates (no timer)


    // a quick software design note here:
    // using one timer for infrequent updates does overload the network when the updates come through, sure
    // but having a timer for each entity (another possible approach) would be far more complex logic-wise
    // if we use a batched messaging system we should avoid the downsides of this approach, and its simple
    public bool IsReadyToSendEntityUpdate(e_possibleentitystates state)
    {
        if (state == e_possibleentitystates.Independent)
        {
            return true;
        } else if (state == e_possibleentitystates.Localized)
        {
            return Time.time > lastLocalizedUpdate + NetworkResources.Instance.localizedEntityPacketFrequency;
        } else if (state == e_possibleentitystates.Influenced)
        {
            return Time.time > lastInfluencedUpdate + NetworkResources.Instance.influencedEntityPacketFrequency;
        } else if (state == e_possibleentitystates.Controlled)
        {
            return Time.time > lastControlledUpdate + NetworkResources.Instance.controlledEntityPacketFrequency;
        }

        return false; // we should never get here
    }


    // called by the server, and the client if the server isn't running on that machine
    public void RemovePlayer(string playerUsername)
    {
        net_connectedclient clientToRemove = GetClientFromUsername(playerUsername);

        if (clientToRemove == null) {return;}// either they're already gone or the username never existed

        onPlayerLeave.Invoke(playerUsername);

        // now send the message that updates all of the other clients
        ServerSenders.Instance.SendPlayerRemoveNotice(GetClientFromUsername(playerUsername).client_index);

        ServerSenders.Instance.SendPlayerKickRequest(playerUsername, "you wanted to leave");
        connectedClients.Remove(clientToRemove);
    }

    public void BeginMulticastBroadcast(ushort port)
    {
        net_serverdata data = new net_serverdata();
        data.name = "super cool server";
        data.ip = util_network.GetLocalIPAddress();
        data.port = port;   
        // broadcast every 2 seconds
        MulticastClient.Instance.PeriodicBroadcast(NetworkResources.lanMulticastAddress, data.Package(), 2);
    }

    public static string[] GetConnectedUsernames()
    {
        return new string[] {"localplayer"};
    }

    public void ChangeClientPermissions(string username, ushort newPermissionLevel)
    {
        if (Instance.isServerActive)
        {
            ServerSenders.Instance.SendPermissionUpdate(GetClientFromUsername(username).client_index, newPermissionLevel);
        } else
        {
            ClientSenders.Instance.SendCommandRequest(cmd_console.GetCommandData("p"), new string[] {username, newPermissionLevel.ToString()});
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
            ClientSenders.Instance.SendCommandRequest(cmd_console.GetCommandData("wlist"), new string[] {username});
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
                ClientSenders.Instance.SendCommandRequest(cmd_console.GetCommandData("blist"), new string[] {username});
            }
        }
    }

    public void KickPlayer(string username)
    {
        if (isServerActive)
        {
            // like all other commands, we assume the local player is an admin because they are literally always an admin
            ServerSenders.Instance.SendPlayerKickRequest(username, "kicked via command");
        } else
        {
            ClientSenders.Instance.SendCommandRequest(cmd_console.GetCommandData("kick"), new string[] {username});
        }
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
        cmd.LogRaw("[Server] Starting server ...", NetworkResources.Instance.serverUpdateColor);
        server.Start(port, maxClientCount);
        BeginMulticastBroadcast(port);
        isServerActive = true;

        // we now have to generate the world
        GameManager.InitializeNewGame(-1); // random world seed
    }

    public void StartMultiplayerServer(ushort clientCount)
    {
        cmd.LogRaw("[Server] Setting up multiplayer server on port " + NetworkResources.defaultServerPort + "...", NetworkResources.Instance.serverUpdateColor);
        StartServer(NetworkResources.defaultServerPort, clientCount);
        ClientNetworkManager.Instance.username = "localplayer";
        ClientNetworkManager.Instance.ConnectToLocalServer();
    }

    // starting a singleplayer world
    public void StartSingleplayerServer()
    {
        cmd.LogRaw("[Server] Setting up singleplayer server on port " + NetworkResources.defaultServerPort + "...", NetworkResources.Instance.serverUpdateColor);
        StartServer(NetworkResources.defaultServerPort, 1);
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

    public void SetControllingEntity(ushort clientId, e_genericentity entity)
    {
        if (clientId == LocalPlayer.localClient.client_index)
        {
            CameraController.SetControlMode(CameraControlMode.None);
        } // unparenting and stopping the camera so that it doesn't end up deleted/disabled

        entity.gameObject.SetActive(true);

        net_connectedclient client = ServerNetworkManager.GetClient(clientId);
        if (client.controllingEntity != null) {client.controllingEntity.onExitControl.Invoke();}

        client.controllingEntity = entity;
        if (clientId == LocalPlayer.localClient.client_index)
        {
            cb_renderingmanager.Instance.RenderFrom(entity.data.GetPosition());
            entity.onEnterControl.Invoke();
        }

        if (ServerNetworkManager.Instance.isServerActive)
        {
            cmd.LogRaw($"[Server] setting client {clientId} control to {entity.gameObject.name}...", NetworkResources.Instance.serverUpdateColor);

            // great, now its done on the server side
            // we still need to update everyone (except the local client, obviously)
            Message toOthers = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.entity_control);

            toOthers.AddInt(clientId);
            toOthers.AddInt(entity.data.index);

            ServerSenders.SendToAllExceptLocal(toOthers);
        } else
        {
            cmd.LogRaw($"[Client] setting client {clientId} control to {entity.gameObject.name}...", NetworkResources.Instance.clientUpdateColor);
        }
    }

    public void StopServer()
    {
        // before actually stopping the server, we kick all clients - telling them the server was closed
        for (int i = 0; i < connectedClients.Count; i++)
        {
            ServerSenders.Instance.SendPlayerKickRequest(connectedClients[i].username, "server closed");
        }
        isServerActive = false;
        server.Stop();
    }

    public void ProcessChatMessage(ushort fromClientId, string msg)
    {
        // show the chat message in a UI
        //Debug.Log("shit" + LocalPlayer.localClient.client_index + "    " + fromClientId);

        ServerSenders.Instance.SendChatMessage(fromClientId, msg);
    }

    public void SystemTeleport(e_genericentity entity, int index)
    {
        if (entity == null) {cmd.Log("you can't system teleport nothing, dipshit"); return;}
        entity.data.SetPosition(cb_solarsystem.Instance.monoBodies[index + 2].pose.data.GetPosition().Add(Vector3.right * (float)((3f + WorldManager.SeaLevelRadius(index + 2) + WorldManager.Instance.GetHeightAtDirection(Vector3.right, index)))));
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
