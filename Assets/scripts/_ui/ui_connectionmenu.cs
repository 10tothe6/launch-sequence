using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.AI;

public class ui_connectionmenu : MonoBehaviour
{
    public ui_list lanGames;


    public TMP_InputField in_port;
    public TMP_InputField in_ip;
    public TMP_InputField in_username;

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
        string[] splitData = util_string.SplitByChar(data,':');

        

        string serverIP = splitData[0];

        if (!util_network.IsValidIP(serverIP))
        {
            return;
        }

        

        ushort port;
        if (!ushort.TryParse(splitData[1], out port))
        {
            return;
        }

        

        string serverName = splitData[2];

        TryAddLANGame(serverIP, port, serverName);
    }

    public void TryAddLANGame(string ip, ushort port, string name)
    {
        if (!gameObject.activeSelf) {return;}
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

           
            GameObject g_newElement = lanGames.AddItem(name);

            // here is where we tell the button component what to do when clicked
            g_newElement.GetComponent<ui_buttondisplay>().AddToOnClick(() => AttemptServerConnection(ip,port));
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

    public void AttemptServerConnection(string ip, ushort port)
    {
        if (in_username.text.Length == 0)
        {
            ui_infoalerts.Instance.ShowFullscreenAlert("please set the username!",Color.purple);
            return;
        } else
        {
            ClientNetworkManager.Instance.username = in_username.text;
        }
        ClientNetworkManager.Instance.ConnectToServer(ip, port);

        // enter into a loading screen where we wait for the connection to go through
        UIManager.Instance.SwitchMenu("connection loading");
    }

    public void AttemptDirectServerConnection()
    {
        string rawPort = in_port.text;
        string rawIP = in_ip.text;

        ushort port;

        bool isDataValid = false;
        if (ushort.TryParse(rawPort, out port))
        {
            if (util_network.IsValidIP(rawIP))
            {
                isDataValid = true;
            }
        }

        if (isDataValid)
        {
            AttemptServerConnection(rawIP,port);
        } else
        {
            ui_infoalerts.Instance.ShowFullscreenAlert("please enter valid server data!",Color.purple);
        }
    }
}
