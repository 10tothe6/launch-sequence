using UnityEngine;

public class cb_mainmenucontroller : MonoBehaviour
{
    private static cb_mainmenucontroller _instance;

    public static cb_mainmenucontroller Instance
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
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public GameObject p_celestialBody;
}
