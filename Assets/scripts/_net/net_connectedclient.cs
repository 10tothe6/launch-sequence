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
    public bool isLocal;
    public e_generic controllingEntity;

    public string username;
    public ushort permissionLevel; // references net_permissionlevel
    public ushort client_index; // THE RIPTIDE INDEX
    // NOT USING CLIENT IDS/INDEXES, USUALLY

    public float ping; // two-way ping

    public net_connectedclient() {}    
    public net_connectedclient(string username, ushort client_index)
    {
        this.username = username;
        this.client_index = client_index;
        this.permissionLevel = 0;
    }

    public net_connectedclient(string username, ushort client_index, ushort permissionLevel)
    {
        this.username = username;
        this.client_index = client_index;
        this.permissionLevel = permissionLevel;
    }

    public string ParseToString()
    {
        string result = "";

        result += username;
        result += ",";
        result += permissionLevel;
        result += ",";
        result += client_index;

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
}
