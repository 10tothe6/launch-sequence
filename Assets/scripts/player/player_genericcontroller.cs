using UnityEngine;
using UnityEngine.Events;

// a generic controller for accepting a player_keypresspacket, 
// which gets then sent to the ACTUAL player controller

// this script doesn't need to do much other than store the packet

public class player_genericcontroller : MonoBehaviour
{
    public player_keypresspacket mostRecentPacket {get; private set;}

    public UnityEvent onPacketUpate;

    public void AcceptKeyPresses(player_keypresspacket packet)
    {
        mostRecentPacket = packet;

        onPacketUpate.Invoke(); // tell whoever that we got key presses
    }
}
