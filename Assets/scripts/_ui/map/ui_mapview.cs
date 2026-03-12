using UnityEngine;

// general script to oversee the map menu's UI

// the actual map objects themselves are managed over at WorldManager.cs,
// this is for specifically UI stuff

public class ui_mapview : MonoBehaviour
{
    private static ui_mapview _instance;

    public static ui_mapview Instance
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

    public Transform t_mapIcons;

    public void SetupBody(cb_trackedbody body, ui_screenspaceicon comp)
    {
        comp.icon = WorldManager.Instance.cbIcons[body.data.bodyType];
        comp.t_uiContainer = t_mapIcons;

        comp.Initialize();
    }
}
