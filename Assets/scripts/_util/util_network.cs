using UnityEngine;

public class util_network
{
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
