using System;
using MetaVoiceChat;
using MetaVoiceChat.NetProviders;
using MetaVoiceChat.Utils;
using UnityEngine;

// since Riptide is not supported by default with Meta VC

public class VcNetworkProvider : MonoBehaviour, INetProvider
{
    bool INetProvider.IsLocalPlayerDeafened => false;

    void Awake()
    {
        GetComponent<e_genericentity>().onEnterControl.AddListener(() =>
        {
            StopClient();
            StartClient();
        });

        StartClient();
    }

    public MetaVc ins_metaVC;

    public MetaVc MetaVc { get; private set; }

    public void StartClient()
    {
        cmd.LogRaw("starting client...", Color.orange);

        static int GetMaxDataBytesPerPacket()
        {
            // maximum possible bytes (ish, the real number for riptide should be 1225 not 1200)
            // I lowered it to give a margin
            // the - 13 is from meta vc, no idea what for
            int bytes = 1200 - 13; 

            bytes -= sizeof(int); // Index
            bytes -= sizeof(double); // Timestamp
            bytes -= sizeof(byte); // Additional latency
            bytes -= sizeof(ushort); // Array length

            return bytes;
        }

        cmd.LogRaw("good", Color.orange);

        MetaVc = ins_metaVC;

        bool isLocalPlayer = false;

        if (LocalPlayer.localClient != null)
        {
            if (LocalPlayer.localClient.controllingEntity != null)
            {
                isLocalPlayer = LocalPlayer.localClient.controllingEntity == GetComponent<e_genericentity>();
            }
        }
        
        MetaVc.StartClient(this, isLocalPlayer, GetMaxDataBytesPerPacket());
    }

    public void StopClient()
    {
        MetaVc.StopClient();
    }


    // i should note that this function is required by MetaVc, in this exact format
    // I'm assuming that this system is similar to the way that I handle console commands,
    // where clients send data to the server and the server relays it to other clients

    // what matters though is that this is the "send" function, and below is the receive function
    void INetProvider.RelayFrame(int index, double timestamp, ReadOnlySpan<byte> data)
    {
        var array = FixedLengthArrayPool<byte>.Rent(data.Length);
        data.CopyTo(array);

        float additionalLatency = Time.deltaTime;
        audio_voiceframe frame = new audio_voiceframe(index, timestamp, additionalLatency, array);

        // this function is ONLY called when the LOCAL entity has data it wants to pass out
        // as such, no matter what, we just have one thing to do
        // send the data to the server
        ClientSenders.Instance.SendVoicePacketToServer(frame);

        FixedLengthArrayPool<byte>.Return(array);
    }
    

    // the following is from the MetaVc example:
    // ***
    // A possible optimization is to use target RPCs and only send filled arrays to clients that are within audible range, and empty arrays to others.
    // Audible range would be determined by the distance between the reciever's position and the sender's audio source position.
    // ***

    public void ReceiveFrame(audio_voiceframe frame)
    {
        if (MetaVc == null)
            return;

        // Don't apply server Time.deltaTime to additionalLatency -- this frame did not go over the network again.
        
    
        if (ServerNetworkManager.Instance.isServerActive)
        {
            //cmd.LogRaw("oofserver", Color.limeGreen);

            float additionalLatency = frame.additionalLatency - Time.deltaTime;
            MetaVc.ReceiveFrame(frame.index, frame.timestamp, additionalLatency, frame.array);
        }
        else
        {
            //cmd.LogRaw("oofclient", Color.limeGreen);

            MetaVc.ReceiveFrame(frame.index, frame.timestamp, frame.additionalLatency, frame.array);
        }
    }
}
