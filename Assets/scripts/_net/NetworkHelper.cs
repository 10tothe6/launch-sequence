using UnityEngine;

// sort of a mish-mash of server and client stuff
// its one level "higher" than ServerNetworkManager.cs and ClientNetworkManager.cs

public class NetworkHelper : MonoBehaviour
{
    private static NetworkHelper _instance;

    public static NetworkHelper Instance
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
    }

    // sets up a single-person server
    public void StartSingleplayerGame()
    {
        
    }
}
