using UnityEngine.UI;
using TMPro;
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

    public GameObject g_playerBars; // container for the below readouts on elec, temp and oxy

    public Sprite[] temperatureChangeIcons;
    public Image temperatureChangeDisplay;

    public TextMeshProUGUI temperatureDisplay;
    public TextMeshProUGUI oxygenDisplay;
    public TextMeshProUGUI electricityDisplay;

    // TODO: not call in update
    void Update()
    {
        if (GameManager.gameState == GameState.InGame)
        {
            g_playerBars.SetActive(true); // TODO: not periodic
            UpdatePlayerBars();
        } else
        {
            g_playerBars.SetActive(false); // TODO: not periodic
        }
    }

    // electricity, temperature, and health right now
    public void UpdatePlayerBars()
    {
        // first, get the values you need
        // TODO: this
        float playerTemperature = 30f; // in degrees celcius
        float playerTempChange = 1; // in deg C/sec

        float playerElectricity = 1000f; // in "units of charge"
        float playerMaxElectricity = 1500f; // in "units of charge"

        float playerOxygen = 5.5f; // in m^3
        float playerMaxOxygen = 20f; // in m^3



        // now to actually display the values
        oxygenDisplay.text = playerOxygen.ToString() + " / " + playerMaxOxygen.ToString();
        electricityDisplay.text = playerElectricity.ToString() + " / " + playerMaxElectricity.ToString();

        temperatureDisplay.text = playerTemperature.ToString() + "°C";

        temperatureChangeDisplay.sprite = temperatureChangeIcons[util_game.FormatTemperatureChange(playerTempChange) + 3];
    }

    public void SetupDebugInfo()
    {
        // the title of the tab
        ui_debugmenu.Instance.AddEntry("[hud]", 
        () => "",
        "game_main");

        ui_debugmenu.Instance.AddEntry("is server", 
        () => ServerNetworkManager.Instance.isServerActive ? "yes" : "no",
        "game_main");

        ui_debugmenu.Instance.AddEntry("camera mode", 
        () => CameraController.controlMode.ToString(),
        "game_main");

        // based on the player's position, what planet's SOI are they in?
        ui_debugmenu.Instance.AddEntry("current SOI", 
        () => (WorldManager.Instance.GetSOIIndex() - 2).ToString(),
        "game_main");

        // ksp's "sea level alt"
        ui_debugmenu.Instance.AddEntry("sea level altitude", 
        () => util_game.FormatRawDistance(WorldManager.Instance.GetSeaLevelAltitudeAsDouble()),
        "game_main");
        // ui_debugmenu.Instance.AddEntry("backend sea level altitude", 
        // () => util_game.FormatDistance(WorldManager.Instance.GetSeaLevelAltitudeAsDouble()),
        // "game_main");
        // similar, but distance to center
        ui_debugmenu.Instance.AddEntry("core altitude", 
        () => util_game.FormatRawDistance(WorldManager.Instance.GetCoreAltitude()),
        "game_main");
        // ui_debugmenu.Instance.AddEntry("backend core altitude", 
        // () => util_game.FormatDistance(WorldManager.Instance.GetCoreAltitudeAsDouble()),
        // "game_main");

        ui_debugmenu.Instance.AddEntry("radar altitude", 
        () =>
        {
            if (LocalPlayer.IsControllingEntity())
            {
               return util_game.FormatRawDistance(WorldManager.Instance.GetCoreAltitude().AsDouble() - WorldManager.Instance.GetHeightAtSurface(LocalPlayer.localClient.controllingEntity.data.GetPosition())); 
            } else
            {
                return "0";
            }
        },
        "game_main");

        ui_debugmenu.Instance.AddEntry("eq radius", 
        () => util_game.FormatRawDistance(WorldManager.SeaLevelRadius()),
        "game_main");

        ui_debugmenu.Instance.AddEntry("entity count", 
        () => EntityManager.Instance.allEntities.Count.ToString(),
        "game_main");
    }
}
