using UnityEngine;


// might refactor this approach, but for now this class will handle in-game ui stuff, generally

public class ui_playerhud : MonoBehaviour
{
    private static ui_playerhud _instance;

    public static ui_playerhud Instance
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


    void Start()
    {
        SetupDebugInfo();
    }
    public void SetupDebugInfo()
    {
        // the title of the tab
        ui_debugmenu.Instance.AddEntry("[hud]", 
        () => "",
        "game_main");

        // based on the player's position, what planet's SOI are they in?
        ui_debugmenu.Instance.AddEntry("current SOI", 
        () => (WorldManager.Instance.GetSOIIndex() - 2).ToString(),
        "game_main");
    }
}
