using TMPro;
using UnityEngine;
using UnityEngine.Events;

// the return of WorldManager.cs!

// controls solar system stuff, craft stuff, etc.

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;

    public static WorldManager Instance
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

    public Sprite[] cbIcons;

    public Transform t_bodyContainer;
    public Transform t_mapBodyContainer;
    public GameObject p_mapBody; // maybe move all this 'map body' stuff into cb_solarsystem??

    // for safekeeping, basically
    public int worldSeed;

    public cb_solarsystem ss;
    // simple parse check for now, may eventually make a more modular system

    // **** map view ****
    
    public float mapScaleFactor; // from real scale to how it appears, usually very low value
    public int mapFocusIndex; // what cb to focus on

    // *********

    // ************ TIME-RELATED STUFF ************

    public float currentTimewarpFactor;

    // ****************************

    public UnityEvent onNewWorldGenerate;

    public void UpdateWorld()
    {
        cb_renderingmanager.Instance.UpdateAllBodyPositions();
    }

    public int GetSOIIndex()
    {
        Vector3 playerPos = Vector3.zero;

        // I can't use the 'getbodypositions' function or whatever bc i have to use the CURRENT position

        float[] gravForces = new float[cb_solarsystem.Instance.monoBodies.Count];
        
        for (int i = 0; i < cb_solarsystem.Instance.monoBodies.Count; i++)
        {
            float dist = Vector3.Distance(
                cb_solarsystem.Instance.monoBodies[i].pose.data.GetPosition().ToVector3(), 
            cb_renderingmanager.Instance.entityInControl.data.GetPosition().ToVector3());
            // big G times big M divided by distance^2
            float force = cb_solarsystem.gravConstant * cb_solarsystem.Instance.monoBodies[i].data.mass / (dist * dist);

            gravForces[i] = force;
        }

        int kingIndex = 0;

        // does not at all require two loops, just doing it this way so its a little nicer to read
        // (splitting the calculation step and sorting step instead of combining them)
        for (int i = 1; i < gravForces.Length; i++)
        {
            if (gravForces[kingIndex] < gravForces[i])
            {
                kingIndex = i;
            }
        }

        return kingIndex;
    }

    public void StartGame(TMP_InputField input)
    {
        int parsed = -1;
        if (int.TryParse(input.text, out parsed))
        {
            StartGame(parsed);
        } else
        {
            StartGame(-1); // will generate a random seed
        }
    }

    // generates a new solar system
    public void StartGame(int worldSeed)
    {
        // -1 means random seed
        if (worldSeed == -1) {worldSeed = util_math.GetRandomInt();}

        this.worldSeed = worldSeed;
        ss.Generate(worldSeed);
        
        CameraController.SetControlMode(CameraControlMode.Freecam);
        UIManager.Instance.EnterMapView();

        Program.gameState = GameState.InGame;

        cb_renderingmanager.Instance.SetupEntities();

        // VERY MUCH TEMP
        cb_renderingmanager.Instance.player.data.localPosition = ss.monoBodies[2].pose.data.localPosition.Add(Vector3.right * 25f);

        onNewWorldGenerate.Invoke();
    }

    // just putting this here for now
    public void SetupMap()
    {
        mapFocusIndex = 1;
        // for the actual game, there are 2 different lists of objects
        // 'bodies' are the actual planet objects
        // 'map bodies' are specifically for the map view

        if (t_mapBodyContainer.childCount != ss.monoBodies.Count)
        {
            RegenerateMapBodies();
        }

        ui_mapview.Instance.SetupDebugInfo();

        RefreshMap();
    }

    public void RefreshMap()
    {
        Vector3[] p = ss.GetBodyPositions(GetMapScaleFromFocusedBody());

        for (int i = 0; i < t_mapBodyContainer.childCount; i++)
        {
            // subtracting the position of the center body focuses on it
            // easier than moving the camera ig?
            t_mapBodyContainer.GetChild(i).GetComponent<cb_mapobject>().Initialize(p[i] - p[mapFocusIndex]);
        }
        for (int i = 0; i < t_mapBodyContainer.childCount; i++)
        {
            t_mapBodyContainer.GetChild(i).localPosition = p[i] - p[mapFocusIndex];
        }
    }

    public void RegenerateMapBodies()
    {
        ui_canvasutils.DestroyChildren(t_mapBodyContainer.gameObject);

        for (int i = 0; i < ss.monoBodies.Count; i++)
        {
            GameObject g_newMapBody = Instantiate(p_mapBody, t_mapBodyContainer);

            // handling the screenspace icon thing
            ui_linkedicon comp = g_newMapBody.GetComponent<ui_linkedicon>();

            // just pass it to the ui script atp
            ui_mapview.Instance.SetupBody(ss.monoBodies[i],comp);
        }
    }

    // like pressing tab in ksp
    public void CycleMapFocus()
    {
        mapFocusIndex++;
        if (mapFocusIndex >= t_mapBodyContainer.childCount)
        {
            mapFocusIndex = 1;
        }

        RefreshMap();
    }

    public float GetMapScaleFromFocusedBody()
    {
        return mapFocusIndex < 2 ? mapScaleFactor / 10000f : mapScaleFactor * 10f;
    }
}
