using UnityEngine;

// could have put this code inside of WorldManager or cb_solarsystem,
// but the former would get way too messy and the latter is specifically for generation/data

// so this script exists now, the equivalent (more or less) of the CBRenderingManager.cs script from earlier

public class cb_renderingmanager : MonoBehaviour
{
    private static cb_renderingmanager _instance;

    public static cb_renderingmanager Instance
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

    public Transform t_bodyContainer; // could access from cb_solarsystem, but a shortcut feels better

    public void UpdateAllBodyPositions()
    {
        
    }
}
