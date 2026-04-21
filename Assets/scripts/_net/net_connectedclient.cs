using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum net_permissionlevel
{
    Admin = 2, // can ban players, use commands, chance wlist and blist
    Operator = 1, // only using commands, no banning or whitelisting
    User = 0, // nothing
}



// ONLY USED SERVER-SIDE

[System.Serializable]
public class net_connectedclient 
{
    public List<float> chatMessageTimes; // ONLY USED BY THE SERVER (for spam kicking)


    public bool isInSandbox; // this is just a shortcut
    
    public bool isLocal; // TODO:
    public e_genericentity controllingEntity;

    public string username;
    public ushort permissionLevel; // references net_permissionlevel
    public ushort client_index; // THE RIPTIDE INDEX
    // NOT USING CLIENT IDS/INDEXES, USUALLY

    public float ping; // two-way ping

    public net_connectedclient() {chatMessageTimes = new List<float>();}    
    public net_connectedclient(string username, ushort client_index)
    {
        chatMessageTimes = new List<float>();
        this.username = username;
        this.client_index = client_index;
        this.permissionLevel = 0;
    }

    public net_connectedclient(string username, ushort client_index, ushort permissionLevel)
    {
        chatMessageTimes = new List<float>();
        this.username = username;
        this.client_index = client_index;
        this.permissionLevel = permissionLevel;
    }

    public void CheckForSpam()
    {
        for (int i = chatMessageTimes.Count - 1; i >= 0; i--)
        {
            if (Time.time - chatMessageTimes[i] > NetworkResources.spamPeriod)
            {
                chatMessageTimes.RemoveAt(i);
            }
        }

        if (chatMessageTimes.Count > NetworkResources.spamMessageCount) // are we still over the limit?
        {
            ServerSenders.Instance.SendPlayerKickRequest(username, "spamming");
        }
    }

    public string ParseToString()
    {
        string result = "";

        result += username;
        result += ",";
        result += permissionLevel;
        result += ",";
        result += client_index;
        if (controllingEntity != null)
        {
            result += ",";
            result += controllingEntity.data.index;
        }

        return result;
    }

    public static net_connectedclient ParseFromString(string raw)
    {
        net_connectedclient result = new net_connectedclient();
        string[] elements = util_string.SplitByChar(raw,',');
        
        result.username = elements[0];
        result.permissionLevel = ushort.Parse(elements[1]);
        // no need for ping
        result.client_index = ushort.Parse(elements[2]);

        if (elements.Length > 3)
        {
            result.controllingEntity = EntityManager.Instance.GetEntityFromIndex(int.Parse(elements[3]));
        }

        return result;
    }

    public static net_connectedclient[] ParseFromStringArray(string[] raw)
    {
        net_connectedclient[] result = new net_connectedclient[raw.Length];
        for (int i = 0; i < raw.Length; i++)
        {
            result[i] = net_connectedclient.ParseFromString(raw[i]);
        }

        return result;
    }

    public bool IsAdmin()
    {
        return permissionLevel > 1;
    }
    public bool CanUseCommands()
    {
        return permissionLevel > 0;
    }

    // ALL THREE OF THESE FUNCTIONS CANNOT RUN ON THE CLIENT SIDE, MUST BE SERVER SIDE
    // ******************************************************

    public void ToggleSandbox()
    {
        if (isInSandbox)
        {
            ExitSandbox();
        } else
        {
            EnterSandbox();
        }
    }
    public void EnterSandbox()
    {
        // first, we spawn a new robot entity in the sandbox to take control of
        e_genericentity newEntity = EntityManager.Instance.SpawnNewEntityInSandbox("robot", num_precisevector3.Zero()).GetComponent<e_genericentity>();

        // then, we take control of it
        ServerNetworkManager.Instance.SetControllingEntity(client_index, newEntity);

        GameManager.SwitchToSandbox();

        // we let everyone know we're in the sandbox
        isInSandbox = true;
    }

    public void ExitSandbox()
    {
        // every client has a freecam entity in the main game, these are not destroyed
        // we just have to find it, and take control



        e_genericentity freecam = EntityManager.Instance.GetEntityFromName("freecam_" + username);
        if (freecam == null)
        {
            // TODO: 
            return;
        }

        // controlling it
        ServerNetworkManager.Instance.SetControllingEntity(client_index, freecam);

        GameManager.SwitchToGame();

        // let everyone know we're not in the sandbox
        isInSandbox = false;
    }

    // ******************************************************
}
