using UnityEngine;

// TODO: replace some of the data inside of ServerNetworkManager.cs with this class

[System.Serializable]
public class net_serverdata
{
    public string name;

    public ushort maxClients;
    public string ip;
    public ushort port;

    public net_serverdata() {}
}
