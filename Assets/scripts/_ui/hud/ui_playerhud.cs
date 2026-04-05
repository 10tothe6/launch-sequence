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

        // ksp's "sea level alt"
        ui_debugmenu.Instance.AddEntry("sea level altitude", 
        () => util_game.FormatRawDistance(WorldManager.Instance.GetSeaLevelAltitude()),
        "game_main");
        ui_debugmenu.Instance.AddEntry("backend sea level altitude", 
        () => util_game.FormatDistance(WorldManager.Instance.GetSeaLevelAltitude()),
        "game_main");
        // similar, but distance to center
        ui_debugmenu.Instance.AddEntry("core altitude", 
        () => util_game.FormatRawDistance(WorldManager.Instance.GetCoreAltitude()),
        "game_main");
        ui_debugmenu.Instance.AddEntry("backend core altitude", 
        () => util_game.FormatDistance(WorldManager.Instance.GetCoreAltitude()),
        "game_main");

        ui_debugmenu.Instance.AddEntry("eq radius", 
        () => util_game.FormatRawDistance(WorldManager.SeaLevelRadius()),
        "game_main");
    }
}
