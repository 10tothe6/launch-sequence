using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

// no net_ prefix bc this is a HL script

// used to communicate information over LAN without having to set an IP address
// make sure firewall lets multicast through !!!!

// this script is meant to be only instanced ONCE in the scene (hence the singleton implementation),
// which is why receiving from many different addresses is supported
// sending to different addresses is not supported because I don't care

public class MulticastClient : MonoBehaviour
{
    private static MulticastClient _instance;

    public static MulticastClient Instance
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

        monitoredAddresses = new List<string>();
        periodicMessages = new List<net_periodicmessage>();
        caughtMessages = new string[0];
    }

    void Start()
    {
        Initialize(54236);
    }

    public bool updatePeriodically; // false if you have another script controlling the updates

    private UdpClient client;
    public static string multicastAddress {get; private set;}
    public static ushort multicastPort {get; private set;}

    public static List<string> monitoredAddresses {get; private set;}

    public UnityEvent<string,string> onReceiveMessage;

    public List<net_periodicmessage> periodicMessages;

    private string[] caughtMessages; // this is dumb

    void OnApplicationQuit()
    {
        client.Close();
    }

    public void PeriodicBroadcast(string ip, string msg, float timing)
    {
        net_periodicmessage newBroadcast = new net_periodicmessage(ip,msg, timing);

        periodicMessages.Add(newBroadcast);
    }

    public void ClearPeriodicBroadcasts()
    {
        periodicMessages.Clear();
    }

    public void Initialize(ushort port)
    {
        multicastPort = port;
        client = new UdpClient(AddressFamily.InterNetwork);
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        client.Client.Bind(new IPEndPoint(IPAddress.Any, port));
    }

    public void AddMonitoredAddress(string address)
    {
        if (client == null) {return;}
        monitoredAddresses.Add(address);
        client.JoinMulticastGroup(IPAddress.Parse(address), 3);
    }

    public void RemoveMonitoredAddress(string address)
    {
        monitoredAddresses.Remove(address);
        client.DropMulticastGroup(IPAddress.Parse(address), multicastPort);
    }

    public void SetSendAddress(string newAddress)
    {
        if (!util_network.IsValidMulticastAddress(newAddress)) {Debug.Log("tried to set multicast address to invalid"); return;}

        // assuming the address is valid...
        multicastAddress = newAddress;
    }

    void Update()
    {
        if (updatePeriodically)
        {
            // TODO: run less periodically
            Task.Run(() => {caughtMessages = UpdateClient();});

            for (int i = 0; i < caughtMessages.Length; i++)
            {
                if (string.IsNullOrEmpty(caughtMessages[i])) {continue;}
                onReceiveMessage.Invoke(monitoredAddresses[i],caughtMessages[i]);
            }

            for (int i = 0; i < periodicMessages.Count; i++)
            {
                if (Time.time > periodicMessages[i].lastBroadcastTime + periodicMessages[i].broadcastFrequency)
                {
                    //cmd.LogRaw("[Server] broadcasting server info...",Color.green);

                    // do the broadcast, and update the last broadcast time
                    SetSendAddress(periodicMessages[i].ip);
                    SendMulticastMessage(periodicMessages[i].message);

                    periodicMessages[i].lastBroadcastTime = Time.time;
                }
            }
        }
    }

    public string[] UpdateClient()
    {
        string[] result = new string[monitoredAddresses.Count];
        // check if any messages have come through on the addresses we're watching out for
        for (int i = 0; i < monitoredAddresses.Count; i++)
        {
            IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, multicastPort);
            byte[] data = client.Receive(ref remoteIP);

            string msg = Encoding.UTF8.GetString(data);

            // we include which address the message came from
            result[i] = msg;
        }
        return result;
    }
    
    public void SendMulticastMessage(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(multicastAddress), multicastPort);

        client.Send(data, data.Length, endPoint);
    }
}
