using UnityEngine;
using UnityEngine.Events;

// the main script that organizes and runs the self-tests (validations)

public class valid_dispatcher : MonoBehaviour
{
    private static valid_dispatcher _instance;
    public static valid_dispatcher Instance
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

    public UnityEvent onFullTest;

    // runs IN GAME, IN AN ACTIVE SERVER (not necessarily YOUR server)
    public void PerformFullSelfTest()
    {
        // all the specific scripts will run based off of this event
        // (easier than having refs)
        onFullTest.Invoke();
    }
}
