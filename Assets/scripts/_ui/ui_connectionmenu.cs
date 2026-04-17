using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

public class ui_connectionmenu : MonoBehaviour
{
    public ui_list lanGames;


    public TMP_InputField in_port;
    public TMP_InputField in_ip;

    public List<net_serverdata> foundServerData;
    // any servers not heard from for more than 10s will be removed
    public List<float> lastHeardFromServers;
    public float serverFadeTime;

    void Awake()
    {
        foundServerData = new List<net_serverdata>();
        lastHeardFromServers = new List<float>();
    }

    void Start()
    {
        // this is the address that games will be sent over, always
        MulticastClient.Instance.AddMonitoredAddress(NetworkResources.lanMulticastAddress);

        MulticastClient.Instance.onReceiveMessage.AddListener(ReceiveMulticastUpdate);
    }

    void Update()
    {
        // TEMP 
        PurgeOldLANGames();
    }

    void PurgeOldLANGames()
    {
        for (int i = lastHeardFromServers.Count-1; i >=0; i--)
        {
            float diff = Time.time - lastHeardFromServers[i];
            if (diff > serverFadeTime)
            {
                lastHeardFromServers.RemoveAt(i);
                foundServerData.RemoveAt(i);
                lanGames.RemoveItemAtIndex(i);
            }
        }
    }

    public void ReceiveMulticastUpdate(string ip, string data)
    {
        string serverIP = ip;

        if (!util_network.IsValidIP(serverIP))
        {
            return;
        }

        string[] splitData = util_string.SplitByChar(data,':');

        ushort port;
        if (!ushort.TryParse(splitData[0], out port))
        {
            return;
        }

        string serverName = splitData[1];

        TryAddLANGame(serverIP, port, serverName);
    }

    public void TryAddLANGame(string ip, ushort port, string name)
    {
        int gameIndex = LANGameIndex(ip);

        if (gameIndex != -1)
        {
            lastHeardFromServers[gameIndex] = Time.time; // refreshing an existing game
        } else
        {
            net_serverdata newServer = new net_serverdata();

            newServer.ip = ip;
            newServer.port = port;
            // no need for maxclients here
            newServer.name = name;

            foundServerData.Add(newServer);
            lastHeardFromServers.Add(Time.time);
        }
    }


    // returns the index, or -1
    int LANGameIndex(string ip)
    {
        for (int i = 0; i < foundServerData.Count; i++)
        {
            if (foundServerData[i].ip == ip)
            {
                return i;
            }
        }

        return -1;
    }

    bool DoesLANGameExist(string ip)
    {
        return LANGameIndex(ip) != -1;
    }

    public void AttemptDirectServerConnection()
    {
        string rawPort = in_port.text;
        string rawIP = in_ip.text;

        ushort port;

        if (ushort.TryParse(rawPort, out port))
        {
            if (util_network.IsValidIP(rawIP))
            {
                ClientNetworkManager.Instance.ConnectToServer(rawIP, port);
            }
        }

        // enter into a loading screen where we wait for the connection to go through
        UIManager.Instance.SwitchMenu("connection loading");
    }
}
