using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class util_network
{
    public static string GetLocalIPAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public static bool IsValidIP(string ip)
    {
        string[] elements = util_string.SplitByChar(ip,'.');
        if (elements.Length != 4)
        {
            return false; // there are fewer/greater than four numbers
        }
        for (int i = 0; i < elements.Length; i++)
        {
            int parsedElement;
            if (!int.TryParse(elements[i], out parsedElement))
            {
                return false; // one of the four numbers isn't a number
            } else
            {
                if (parsedElement > 255 || parsedElement < 0)
                {
                    return false; // outside valid address range
                }
            }
        }

        return true;
    }

    public static bool IsValidMulticastAddress(string raw)
    {
        if (!IsValidIP(raw)) {return false;}
        string[] elements = util_string.SplitByChar(raw, '.');
        
        int firstAddress = int.Parse(elements[0]);
        if (firstAddress < 224 || firstAddress > 239) // multicast ip range is 224.0.0.0 to 239.255.255.255 (i think)
        {
            return false;
        }

        return true;
    }


    public static string RemovePortFromIp(string raw)
    {
        int colonIndex = raw.IndexOf(':');

        if (colonIndex == -1)
        {
            return raw;
        } else
        {
            return raw.Substring(0, colonIndex);
        }
    }
}
