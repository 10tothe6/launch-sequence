using UnityEngine;

// a script exclusively dedicated to network handlers for the CLIENT

public class ClientHandlers : MonoBehaviour
{
    private static ClientHandlers _instance;

    public static ClientHandlers Instance
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
}
