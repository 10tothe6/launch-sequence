using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
    }

    public bool updatePeriodically; // false if you have another script controlling the updates

    private UdpClient client;
    public static string multicastAddress {get; private set;}
    public static ushort multicastPort {get; private set;}

    public static List<string> monitoredAddresses {get; private set;}

    public UnityEvent<string,string> onReceiveMessage;

    public void Initialize(ushort port)
    {
        multicastPort = port;
        client = new UdpClient(multicastPort);
    }

    public void AddMonitoredAddress(string address)
    {
        monitoredAddresses.Add(address);
        client.JoinMulticastGroup(IPAddress.Parse(address), multicastPort);
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
            UpdateClient();
        }
    }

    public void UpdateClient()
    {
        // check if any messages have come through on the addresses we're watching out for
        for (int i = 0; i < monitoredAddresses.Count; i++)
        {
            IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, multicastPort);
            byte[] data = client.Receive(ref remoteIP);

            string msg = Encoding.UTF8.GetString(data);

            // we include which address the message came from
            onReceiveMessage.Invoke(monitoredAddresses[i],msg);
        }
    }
    
    public void SendMulticastMessage(string msg)
    {
        client.Ttl = 5;

        byte[] data = Encoding.UTF8.GetBytes(msg);

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(multicastAddress), multicastPort);

        client.Send(data, data.Length, endPoint);
    }
}
