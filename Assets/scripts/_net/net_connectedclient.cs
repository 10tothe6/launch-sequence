using UnityEngine;

[System.Serializable]
public class net_connectedclient
{
    public string username;
    public ushort client_index; // THE RIPTIDE INDEX
    // NOT USING CLIENT IDS/INDEXES, USUALLY

    public float ping; // two-way ping

    public net_connectedclient(string username, ushort client_index)
    {
        this.username = username;
        this.client_index = client_index;
    }
}
