using UnityEngine;

public class crft_antenna : MonoBehaviour
{
    public float antenna_range;



    // when I say 'ping' I mean the signals that can be picked up by the player's scanner
    [Header("ping settings")]
    [SerializeField]
    private bool is_emitting_ping;
    public float ping_frequency;
    public float ping_range;

    public bool IsEmittingPing()
    {
        // TODO: also check for electricity
        return is_emitting_ping;
    }
}
