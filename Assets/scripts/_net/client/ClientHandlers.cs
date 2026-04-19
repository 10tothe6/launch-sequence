using System.Linq;
using Riptide;
using UnityEngine;

// a script exclusively dedicated to network handlers for the CLIENT


// YOU ARE ENTERING THE HANDLER ZONE vvvv
// ********************************************************

    

    


public class ClientHandlers : MonoBehaviour
{
    private static ClientHandlers _instance;

    public static ClientHandlers Instance
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






    [MessageHandler((ushort)ServerToClientId.entity_position_updates)]
    private static void HandleEntityPositionUpdates(Message message)
    {
        int[] entityIndices = message.GetInts();
        string[] entityPositions = message.GetStrings();

        //cmd.LogRaw($"[Client] got entity position update for {entityIndices.Length} entities.", Color.yellow);

        for (int i = 0; i < entityIndices.Length; i++)
        {
            if (EntityManager.Instance.GetEntityFromIndex(entityIndices[i]) == null) {continue;}
            EntityManager.Instance.GetEntityFromIndex(entityIndices[i]).data.SetPosition(num_precisevector3.FromString(entityPositions[i]));
        }
    }

    [MessageHandler((ushort)ServerToClientId.entity_control)]
    private static void HandleEntityControlChange(Message message)
    {
        int clientIndex = message.GetInt();
        int entityIndex = message.GetInt();

        //Debug.Log(entityIndex);
        
        ServerNetworkManager.Instance.SetControllingEntity((ushort)clientIndex, EntityManager.Instance.GetEntityFromIndex(entityIndex));
    }

    [MessageHandler((ushort)ServerToClientId.kick_player)]
    private static void HandleKick(Message message)
    {
        string reason = message.GetString();

        cmd.LogRaw($"[Client] We were kicked from the server!. Reason: {reason}", Color.yellow);
        
        // TODO: clear any remaining entities and data

        if (ServerNetworkManager.Instance.isServerActive)
        {
            // if we're running the server, we have to stop the server because we (the host) just left)
            ServerNetworkManager.Instance.StopServer();
        }


        ClientNetworkManager.Instance.client.Disconnect();

        GameManager.SwitchToConnectionMenu();
    }

    [MessageHandler((ushort)ServerToClientId.spawn_entity)]
    private static void HandleSpawnEntity(Message message)
    {
        string data = message.GetString();
        int index = message.GetInt();

        cmd.LogRaw($"[Client] Spawning new entity...", Color.yellow);
        EntityManager.Instance.SpawnNewEntity(index, data);
    }

    [MessageHandler((ushort)ServerToClientId.join_denied)]
    private static void HandleJoinDenial(Message message)
    {
        string reason = message.GetString();

        cmd.LogRaw($"[Client] Join request denied by server. Reason: {reason}", Color.yellow);
        ui_infoalerts.Instance.ShowFullscreenAlert($"[Client] Join request denied by server. Reason: {reason}");
        
        ClientNetworkManager.Instance.client.Disconnect();
        UIManager.Instance.SwitchMenu("join server menu");
    }

    [MessageHandler((ushort)ServerToClientId.join_permitted)]
    private static void HandleJoinConfirmation(Message message)
    {
        string[] rawClientData = message.GetStrings();
        string[] rawEntityData1 = message.GetStrings();
        int[] rawEntityData2 = message.GetInts();
        int worldSeed = message.GetInt();

        cmd.LogRaw($"[Client] Setting entity list ({rawEntityData1.Length})...");

        if (!ServerNetworkManager.Instance.isServerActive)
        {
            GameManager.InitializeNewGame(worldSeed);
            
            for (int i = 0; i < rawEntityData1.Length; i++)
            {
                // first, make the new prefab
                EntityManager.Instance.SpawnNewEntity(rawEntityData2[i], rawEntityData1[i]); 
            }
        }
        
        net_connectedclient[] clientData = net_connectedclient.ParseFromStringArray(rawClientData);

        if (!ServerNetworkManager.Instance.isServerActive)
        {
            cmd.LogRaw($"[Client] Join request accepted. Setting client list ({rawClientData.Length})...", Color.yellow);
            ServerNetworkManager.Instance.connectedClients = clientData.ToList();
        } else
        {
            cmd.LogRaw($"[Client] Join request accepted. Client list skipped cuz we're a server", Color.yellow);
        }
        
        LocalPlayer.localClient = ServerNetworkManager.GetClient(ClientNetworkManager.Instance.client.Id);

        ServerNetworkManager.Instance.onJoinServer.Invoke();

        // saying the local player has joined the game
        ui_chat.Instance.AddChatMessage($"{LocalPlayer.localClient.username} joined the game", Color.yellow);
    }

    [MessageHandler((ushort)ServerToClientId.player_connected)]
    private static void HandleNewPlayer(Message message)
    {
        net_connectedclient newPlayer = net_connectedclient.ParseFromString(message.GetString());
        
        if (!ServerNetworkManager.Instance.isServerActive)
        {
            ServerNetworkManager.Instance.connectedClients.Add(newPlayer);
        }
        ServerNetworkManager.Instance.onPlayerJoin.Invoke(newPlayer.username);


        // saying a new player has joined the game
        ui_chat.Instance.AddChatMessage($"{newPlayer.username} joined the game", Color.yellow);
    }

    [MessageHandler((ushort)ServerToClientId.chat_message_update)]
    private static void HandleIncomingChatMessage(Message message)
    {
        cmd.LogRaw($"[Client] got chat message!", Color.yellow);

        ushort senderId = message.GetUShort();
        string data = message.GetString();

        ui_chat.Instance.AddChatMessage($"<{ServerNetworkManager.GetClient(senderId).username}> " + data);
    }

    [MessageHandler((ushort)ServerToClientId.player_disconnected)]
    private static void HandlePlayerDisconnect(Message message)
    {
        string playerUsername = message.GetString();
        if (!ServerNetworkManager.Instance.isServerActive)
        {
            // we only remove the player from the clients list if the server hasn't already done so
            ServerNetworkManager.Instance.RemovePlayer(playerUsername);
        }

        // saying whatever player
        ui_chat.Instance.AddChatMessage($"{playerUsername} left the game", Color.yellow);
    }
}
