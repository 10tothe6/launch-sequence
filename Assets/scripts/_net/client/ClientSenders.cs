using Riptide;
using UnityEngine;

public class ClientSenders : MonoBehaviour
{
    private static ClientSenders _instance;

    public static ClientSenders Instance
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




    public void SendKeyPressesToServer()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.key_presses);
        message.AddString(Input.GetKeypressPacket().ParseToString());
        ClientNetworkManager.Instance.client.Send(message);
    }

    public void SendJoinRequestToServer()
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.join_request);
        message.AddString(ClientNetworkManager.Instance.username);
        message.AddString(Program.Instance.version);
        ClientNetworkManager.Instance.client.Send(message);
    }

    public void SendChatMessageToServer(string msg)
    {
        cmd.LogRaw($"[Client] sending chat message to server...", Color.yellow);
        
        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.chat_message_send);
        
        // no need for any id/credentials because we can see the fromClientId on the other end
        message.AddString(msg);
        // TODO: timestamp?

        ClientNetworkManager.Instance.client.Send(message);
    }

    public void SendCommandRequest(cmd_consolecommand command, string[] args)
    {
        cmd.LogRaw($"[Client] requesting {command.names[0]} command from server...", Color.yellow);

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.command_request);

        message.AddString(command.names[0]);
        message.AddStrings(args);

        ClientNetworkManager.Instance.client.Send(message);
    }

    public void SendLeaveRequest()
    {
        cmd.LogRaw($"[Client] requesting to leave the server...", Color.yellow);

        Message message = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.leave_request);

        // no args necessary for now

        ClientNetworkManager.Instance.client.Send(message);
    }
}
