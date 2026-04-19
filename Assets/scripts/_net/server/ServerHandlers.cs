using Riptide;
using UnityEngine;

public class ServerHandlers : MonoBehaviour
{
    private static ServerHandlers _instance;

    public static ServerHandlers Instance
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




    [MessageHandler((ushort)ClientToServerId.join_request)]
    private static void HandleJoinRequest(ushort fromClientId, Message message)
    {
        string username = message.GetString();
        string version = message.GetString();

        cmd.LogRaw($"[Server] Received join request from '{username}'. Validating...", Color.cyan);        

        if (ServerNetworkManager.Instance.IsUsernameTaken(username))
        {
            ServerSenders.Instance.SendJoinDenial(fromClientId, "duplicate username");
            cmd.LogRaw($"[Server] Client denied. Reason: duplicate username.", Color.cyan);
        } 
        else if (Program.Instance.version != version)
        {
            ServerSenders.Instance.SendJoinDenial(fromClientId, "wrong version");
            cmd.LogRaw($"[Server] Client denied. Reason: wrong game version.", Color.cyan);
        }

        
        else
        {
            bool passedListCheck = false;

            if (ServerNetworkManager.Instance.useWhitelist)
            {
                // whitelist check

                // TODO: use the GetIP() function
                if (ServerNetworkManager.Instance.whitelist.Contains(util_network.RemovePortFromIp(ServerNetworkManager.Instance.server.Clients[fromClientId-1].ToString())))
                {
                    // on the whitelist, so we good
                    passedListCheck = true;
                } else
                {
                    ServerSenders.Instance.SendJoinDenial(fromClientId, "not whitelisted");
                    cmd.LogRaw($"[Server] Client denied. Reason: not on whitelist.", Color.cyan);
                }
            } else
            {
                // blacklist check

                // TODO: use the GetIP() function
                if (!ServerNetworkManager.Instance.blacklist.Contains(util_network.RemovePortFromIp(ServerNetworkManager.Instance.server.Clients[fromClientId-1].ToString())))
                {
                    // not on blacklist, so we good
                    passedListCheck = true;
                } else
                {
                    ServerSenders.Instance.SendJoinDenial(fromClientId, "blacklisted");
                    cmd.LogRaw($"[Server] Client with denied. Reason: on blacklist.", Color.cyan);
                }
            }

            if (passedListCheck)
            {
                cmd.LogRaw($"[Server] Client accepted.", Color.cyan);

                net_connectedclient newClient = new net_connectedclient(username, fromClientId);

                ServerNetworkManager.Instance.connectedClients.Add(newClient);


                // okay yeah this assumes the first client is the local one, but when ISN'T that the case?
                if (LocalPlayer.localClient == null) 
                {
                    LocalPlayer.localClient = newClient;

                    // default permissions
                    newClient.permissionLevel = 2;
                } else
                {
                    newClient.permissionLevel = 0;
                }
                ServerSenders.Instance.SendJoinConfirm(fromClientId);
            }
        }
    }


    [MessageHandler((ushort)ClientToServerId.chat_message_send)]
    private static void HandleChatMessage(ushort fromClientId, Message message)
    {
        string msg = message.GetString();
        ServerNetworkManager.GetClient(fromClientId).chatMessageTimes.Add(Time.time); // keeping track of when they send the message
        if (ServerNetworkManager.GetClient(fromClientId).chatMessageTimes.Count > NetworkResources.spamMessageCount)
        {
            ServerNetworkManager.GetClient(fromClientId).CheckForSpam();
        }
        ServerNetworkManager.Instance.ProcessChatMessage(fromClientId, msg);
    }

    [MessageHandler((ushort)ClientToServerId.leave_request)]
    private static void HandleLeaveRequest(ushort fromClientId, Message message)
    {
        ServerNetworkManager.Instance.RemovePlayer(ServerNetworkManager.GetClient(fromClientId).username);
    }

    [MessageHandler((ushort)ClientToServerId.command_request)]
    private static void HandleCommandRequest(ushort fromClientId, Message message)
    {
        string cmdName = message.GetString();
        string[] args = message.GetStrings();

        bool arePermissionsValid = false;

        if (cmd_console.GetCommandData(cmdName).needsAdmin)
        {
            arePermissionsValid = ServerNetworkManager.GetClient(fromClientId).IsAdmin();
        } else
        {
            arePermissionsValid = ServerNetworkManager.GetClient(fromClientId).CanUseCommands();
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
            cmd_console.Instance.ProcessMessage(constructedMessage, fromClientId);
        } else
        {
            cmd.LogRaw($"[Server] blocked command ({cmdName}) request from client {fromClientId} cuz perms", Color.cyan);
        }
    }
}
