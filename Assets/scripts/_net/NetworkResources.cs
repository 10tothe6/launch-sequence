using UnityEngine;

// variables/data for the network

public class NetworkResources : MonoBehaviour
{
    private static NetworkResources _instance;

    public static NetworkResources Instance
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


        lanMulticastAddress = ins_lanMulticastAddress;
        defaultServerPort = ins_defaultServerPort;

        spamPeriod = ins_spamPeriod;
        spamMessageCount = ins_spamMessageCount;
    }

    public int ins_spamMessageCount; // how many messages to send in order to get kicked
    public static int spamMessageCount;

    public float ins_spamPeriod; // in how much time do those messages have to be 
    public static float spamPeriod;




    public Sprite[] permissionLevelIcons;
    public string ins_lanMulticastAddress;
    public static string lanMulticastAddress;

    public ushort ins_defaultServerPort;
    public static ushort defaultServerPort;
}
