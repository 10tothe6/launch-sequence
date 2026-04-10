using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class MulticastSender
{
    public void Run()
    {
        string multicastIP = "239.0.0.222";
        int port = 5000;

        UdpClient client = new UdpClient();
        client.Ttl = 5;

        string msg = "Hello receivers!";
        byte[] data = Encoding.UTF8.GetBytes(msg);

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(multicastIP), port);

        client.Send(data, data.Length, endPoint);
    }
}
