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
    void Start()
    {
        // EntityManager.Instance.onDestroyEntity.AddListener(UpdateVisibleEntities);
        // EntityManager.Instance.onSpawnEntity.AddListener(UpdateVisibleEntities);
    }

    public Transform t_bodyIcons;
    public Transform t_playerIcons;
    public Transform t_entityIcons;

    public GameObject p_entityIcon;

    public num_precisevector3 ConvertPosition(num_precisevector3 worldPosition)
    {
        num_precisevector3 offset = worldPosition.Sub(cb_solarsystem.Instance.monoBodies[WorldManager.Instance.mapFocusIndex].data.pConfig.GetPosition());
        offset = offset.Div((double)(1 / WorldManager.Instance.GetMapScaleFromFocusedBody()));

        return offset;
    }

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
        comp.t_uiContainer = t_bodyIcons;

        comp.Initialize();
        comp.button.onClick.AddListener(() => WorldManager.Instance.SetMapFocus(body.data.pConfig.selfIndex));
    }

    public void UpdatePlayers()
    {
        if (!ClientNetworkManager.Instance.isClientActive) {return;}

        for (int i = 0; i < ServerNetworkManager.Instance.connectedClients.Count; i++)
        {
            if (!ui_canvasutils.HasChildOfName(t_playerIcons.gameObject, ServerNetworkManager.Instance.connectedClients[i].username))
            {
                GameObject g_newIcon = Instantiate(p_entityIcon, t_playerIcons);

                g_newIcon.name = ServerNetworkManager.Instance.connectedClients[i].username;

                e_mapentity comp = g_newIcon.GetComponent<e_mapentity>();

                // temp temp temp
                comp.reference = ServerNetworkManager.Instance.connectedClients[i].controllingEntity;
                comp.showName = true;

                comp.Initialize();
            }
        }
    }

    public void UpdateVisibleEntities()
    {
        
    }
}
