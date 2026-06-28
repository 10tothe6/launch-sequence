using UnityEngine;

[System.Serializable]
public class audio_voiceframe
{
    public int index;
    public double timestamp;
    public float additionalLatency;
    public byte[] array;

    public audio_voiceframe() {}

    public audio_voiceframe(int index, double timestamp, float additionalLatency, byte[] array)
    {
        this.index = index;
        this.timestamp = timestamp;
        this.additionalLatency = additionalLatency;
        this.array = array;
    }
}
