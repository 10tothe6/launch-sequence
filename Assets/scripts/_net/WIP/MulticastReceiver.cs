using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

// ip range for multicast: 224.0.0.0 -> 239.255.255.255
// make sure firewall lets multicast through

public class MulticastReceiver
{
    public void Run()
    {
        int port = 5000;
        string multicastIP = "239.0.0.222";

        UdpClient client = new UdpClient(port);

        client.JoinMulticastGroup(IPAddress.Parse(multicastIP), port);

        // ew
        while (true)
        {
            IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, port);
            byte[] data = client.Receive(ref remoteIP);

            string msg = Encoding.UTF8.GetString(data);
            // done
        }
    }
}
