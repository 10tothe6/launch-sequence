using UnityEngine;

public enum net_permissionlevel
{
    Admin = 2, // can ban players, use commands, chance wlist and blist
    Operator = 1, // only using commands, no banning or whitelisting
    User = 0, // nothing
}

[System.Serializable]
public class net_connectedclient
{
    public string username;
    public ushort client_index; // THE RIPTIDE INDEX
    // NOT USING CLIENT IDS/INDEXES, USUALLY

    public float ping; // two-way ping

    public ushort permissionLevel; // references net_permissionlevel

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

    public bool CanUseCommands()
    {
        return permissionLevel > 0;
    }

    public bool IsAdmin()
    {
        return permissionLevel > 1;
    }
}
