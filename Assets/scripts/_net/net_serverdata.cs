using UnityEngine;

// TODO: replace some of the data inside of ServerNetworkManager.cs with this class

[System.Serializable]
public class net_serverpreviewdata
{
    public string name;

    public ushort maxClients;
    public string ip;
    public ushort port;

    public net_serverpreviewdata() {}

    public string Package()
    {
        string result = ""; 

        result += ip;
        result += ":";
        result += port;
        result += ":";
        result += name;
        // no max clients

        return result;
    }
}
