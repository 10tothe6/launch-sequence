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
    }

    public Sprite[] permissionLevelIcons;
}
