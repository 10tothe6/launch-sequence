using System.Collections.Generic;
using Riptide;
using UnityEngine;

public class ServerSenders : MonoBehaviour
{
    private static ServerSenders _instance;

    public static ServerSenders Instance
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

    public void SendPermissionUpdate(ushort clientIndex, ushort newPerm)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.player_permission_update);
        message.AddUShort(clientIndex);
        message.AddUShort(newPerm);

        ServerNetworkManager.Instance.server.SendToAll(message);
    }

    public void SendKillEntity(int index)
    {
        Message kill = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.kill_entity);

        kill.AddInt(index);

        // because this is only run on the server, we can assume the local client has been updated already
        SendToAllExceptLocal(kill);
    }

    public void SendPlayerKickRequest(string username, string reason)
    {
        Message kickRequest = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.kick_player);

        kickRequest.AddString(reason);

        // the nice part about the 'transient' entity system (see EntityManager.cs) is that I don't need to delete any entities
        // the robot itself just isn't controlled anymore
        // sure, I COULD delete the freecams but I don't have to

        ushort kickedPlayerIndex = ServerNetworkManager.GetClientFromUsername(username).client_index;
        ServerNetworkManager.Instance.server.Send(kickRequest, kickedPlayerIndex);

        SendPlayerRemoveNotice(kickedPlayerIndex);
    }

    public void SendPlayerRemoveNotice(ushort indexOfPlayerRemoved)
    {
        Message noticeOfRemovedPlayer = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.player_disconnected);

        // order is [username, reason]

        // the username of the kicked player is all that we need, other info should already be on the client's side
        noticeOfRemovedPlayer.AddString(ServerNetworkManager.GetClient(indexOfPlayerRemoved).username); 
        noticeOfRemovedPlayer.AddString("kicked by the server");
        
        SendToAllExcept(indexOfPlayerRemoved, noticeOfRemovedPlayer);
    }

    public void SendNewEntity(GameObject g_new)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.spawn_entity);

        // the prefab index is stored in the int, anything else we need is in the string
        e_genericentity comp = g_new.GetComponent<e_genericentity>();
        message.AddString(comp.data.GetRawPackagedData());
        message.AddInt(comp.data.entityPrefabIndex);

        cmd.LogRaw($"[Server] spawning new {g_new.name} entity...", NetworkResources.Instance.serverUpdateColor);

        SendToAllExceptLocal(message);
    }

    // can't pass the index to the client bc they don't know what that is
    public void SendChatMessage(ushort originalSenderId, string msg)
    {
        cmd.LogRaw($"[Server] updating clients with new chat message...", NetworkResources.Instance.serverUpdateColor);

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.chat_message_update);
        message.AddUShort(originalSenderId);
        message.AddString(msg);

        ServerNetworkManager.Instance.server.SendToAll(message);
    }

    public void SendJoinDenial(ushort toClientId, string reason)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.join_denied);
        message.AddString(reason);
        ServerNetworkManager.Instance.server.Send(message, toClientId);

        // nobody else really needs to see that someone tried to join
    }

    public void SendJoinConfirm(ushort toClientId)
    {
        cmd.LogRaw($"[Server] Sending player list to new client...", NetworkResources.Instance.serverUpdateColor);

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.join_permitted);

        // converting the connected client list to a string, then shipping it over
        string[] connectedClients = new string[ServerNetworkManager.Instance.connectedClients.Count];
        for (int i = 0; i < connectedClients.Length; i++)
        {
            connectedClients[i] = ServerNetworkManager.Instance.connectedClients[i].ParseToString();
        }
        message.AddStrings(connectedClients);

        
        net_packagedentitydata[] entityData = EntityManager.Instance.PackageAllEntityData();

        string[] data1 = net_packagedentitydata.MakeDataArray(entityData);
        int[] data2 = net_packagedentitydata.MakeIndexArray(entityData);

        message.AddStrings(data1);
        message.AddInts(data2);
        message.AddInt(WorldManager.Instance.worldSeed);
       
        ServerNetworkManager.Instance.server.Send(message, toClientId);

        // so the one client has been confirmed, great
        // now we tell everyone else that there's a new player
        Message toOthers = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.player_connected);

        // the username and permission level (net_connectedclient) get sent over
        toOthers.AddString(ServerNetworkManager.GetClient(toClientId).ParseToString());
        int otherPlayerCount = ServerNetworkManager.Instance.connectedClients.Count - 1;
        cmd.LogRaw($"[Server] Sending player join notification to {otherPlayerCount} other clients...", NetworkResources.Instance.serverUpdateColor);
        SendToAllExcept(toClientId, toOthers);

        // temp temp temp temp temp
        EntityManager.Instance.PutClientInRobot(toClientId);
        // putting the player in the first planetary system (also temp)
        ServerNetworkManager.Instance.SystemTeleport(ServerNetworkManager.GetClient(toClientId).controllingEntity, 0);
    }

    // the function originally took in an int[] for the entity ids,
    // we have replaced it with an e_entityupdatepackage that has all the data in it
    // SORTING IS DONE ON THE FUNCTION THAT CALLS THIS ONE, NOT DONE HERE
    public void SendEntityPositionUpdates(e_entityupdatepackage package)
    {
        // we're using MULTIPLE messages here, because not all clients need all the data
        // all messages use the entity_position_updates index
        // we obviously don't need to update the server's client
        Message toAll = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.entity_position_updates);

        toAll.AddInts(package.independentIndices);
        // independent goes to all clients, always
        // entity system V1 has only independent, nothing else
        toAll.AddStrings(package.independentData); 
        
        SendToAllExceptLocal(toAll);
    }



    public static void SendToAllExcept(ushort except, Message msg)
    {
        for (int i = 0; i < ServerNetworkManager.Instance.connectedClients.Count; i++)
        {
            if (ServerNetworkManager.Instance.connectedClients[i].client_index == except) {continue;}
            ServerNetworkManager.Instance.server.Send(msg, (ushort)ServerNetworkManager.Instance.connectedClients[i].client_index);
        }
    }

    public static void SendToAllExceptLocal(Message msg)
    {
        int localIndex = -1; // this effectively counts as nothing (no clients have index of -1)
        if (LocalPlayer.localClient != null)
        {
            localIndex = LocalPlayer.localClient.client_index;
        }

        for (int i = 0; i < ServerNetworkManager.Instance.connectedClients.Count; i++)
        {
            if (ServerNetworkManager.Instance.connectedClients[i].client_index == localIndex) {continue;}
            Debug.Log(i);
            ServerNetworkManager.Instance.server.Send(msg, (ushort)ServerNetworkManager.Instance.connectedClients[i].client_index);
        }
    }
}
