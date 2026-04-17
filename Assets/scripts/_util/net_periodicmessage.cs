using UnityEngine;

[System.Serializable]
public class net_periodicmessage
{
    public string ip;
    public string message;
    public float broadcastFrequency;
    public float lastBroadcastTime;

    public net_periodicmessage() {}
    public net_periodicmessage(string ip, string message, float broadcastFrequency)
    {
        this.ip = ip;
        this.message = message;
        this.broadcastFrequency = broadcastFrequency;
    }
}
