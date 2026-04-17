using UnityEngine;

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
    }

    public Sprite[] permissionLevelIcons;
    public string ins_lanMulticastAddress;
    public static string lanMulticastAddress;
}
