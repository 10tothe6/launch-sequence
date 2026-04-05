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

    public void SetupDebugInfo()
    {
        // the title of the tab
        ui_debugmenu.Instance.AddEntry("[map]", 
        () => "",
        "map");

        ui_debugmenu.Instance.AddEntry("dist from focus", 
        () => Vector3.Distance(CameraController.t_cam.position, WorldManager.Instance.GetMapBasePosition()).ToString(),
        "map");

        ui_debugmenu.Instance.AddEntry("name", 
        () => cb_solarsystem.Instance.monoBodies[WorldManager.Instance.mapFocusIndex].data.name,
        "map");
        ui_debugmenu.Instance.AddEntry("planet index", 
        () => WorldManager.Instance.mapFocusIndex.ToString(),
        "map");

        ui_debugmenu.Instance.AddEntry("body type", 
        () => cb_solarsystem.BodyType(cb_solarsystem.Instance.monoBodies[WorldManager.Instance.mapFocusIndex].data.bodyType),
        "map");
        ui_debugmenu.Instance.AddEntry("moon count", 
        () => cb_solarsystem.Instance.monoBodies[WorldManager.Instance.mapFocusIndex].GetMoonCount().ToString(),
        "map");

        ui_debugmenu.Instance.AddEntry("has surface", 
        () => cb_solarsystem.Instance.monoBodies[WorldManager.Instance.mapFocusIndex].data.hasSurface ? "Yes" : "No",
        "map");
        ui_debugmenu.Instance.AddEntry("has atmo", 
        () => cb_solarsystem.Instance.monoBodies[WorldManager.Instance.mapFocusIndex].data.hasAtmosphere ? "Yes" : "No",
        "map");

        ui_debugmenu.Instance.AddEntry("map eq radius", 
        () => util_game.FormatRawDistance(WorldManager.SeaLevelRadius(WorldManager.Instance.mapFocusIndex)),
        "map");
    }

    public void SetupBody(cb_trackedbody body, ui_linkedicon comp)
    {
        comp.icon = WorldManager.Instance.cbIcons[body.data.bodyType];
        comp.t_uiContainer = t_mapIcons;

        comp.Initialize();
        comp.button.onClick.AddListener(() => WorldManager.Instance.SetMapFocus(body.data.pConfig.selfIndex));
    }
}
