using TMPro;
using UnityEngine;

// commands, etc.
// in-game chat will be handled differently, unlike minecraft where they are the same window

public class ui_console : MonoBehaviour
{
    private static ui_console _instance;
    public static ui_console Instance
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

    public TMP_InputField consoleInput;
}
