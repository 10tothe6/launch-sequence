using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// the return of WorldManager.cs!

// controls solar system stuff, craft stuff, etc.


// TODO: move the map stuff (mapFocusIndex) to another script
// TODO: PLEASE DO THIS THIS SCRIPT IS BASICALLY A GARBADGE DUMP NOW

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
    public Color[] cbIconColors;
    [Space(20)]
    // TODO: maybe not split these two up?
    public Sprite[] otherIcons;
    

    public Transform t_bodyContainer;
    public Transform t_mapBodyContainer;
    public GameObject p_mapBody; // maybe move all this 'map body' stuff into cb_solarsystem??

    // for safekeeping, basically
    public int worldSeed;

    public cb_solarsystem ss;
    // simple parse check for now, may eventually make a more modular system

    // **** map view ****
    
    public float mapScaleFactorSolar; // from real scale to how it appears, usually very low value
    public float mapScaleFactorPlanetary;
    public int mapFocusIndex; // what cb to focus on

    public float mapLerpSpeed;
    public int oldMapFocusIndex;
    private Vector3[] p;
    private Vector3 mapBasePosition;

    // *********

    // ************ TIME-RELATED STUFF ************

    public float currentTimewarpFactor;

    // ****************************

    public UnityEvent onNewWorldGenerate;
    public Perlin perlin = new Perlin();

    // very much temp
    public GameObject p_robotIcon;
    public Transform t_mapIconContainer;
    public GameObject p_robot;

    // spawns a new player robot at the given position
    // this requires a bit of extra logic to make sure the robot also has a map view icon
    public void SpawnRobot(num_precisevector3 position)
    {
        GameObject g_newRobot = Instantiate(p_robot, null);
        GameObject g_newMapIcon = Instantiate(p_robotIcon, t_mapIconContainer);

        
    }

    void Update()
    {
        RefreshMap();
        UpdateMapBasePosition();
    }

    public void UpdateWorld()
    {
        CameraController.Instance.UpdateCamera();
        cb_renderingmanager.Instance.UpdateAllBodyPositions();

        // doing chunk updates from here, 'update periodically' has been disabled
        for (int i = 0; i < cb_solarsystem.Instance.monoBodies.Count; i++)
        {
            cbt_meshbody comp = cb_solarsystem.Instance.monoBodies[i].GetComponent<cbt_meshbody>();
            if (comp != null)
            {
                comp.UpdateAllChunks();
            }
        }
    }

    public static float SeaLevelRadius()
    {
        return cb_solarsystem.Instance.monoBodies[Instance.GetSOIIndex()].data.tConfig.equitorialRadius;
    }
    public static float SeaLevelRadius(int index)
    {
        return cb_solarsystem.Instance.monoBodies[index].data.tConfig.equitorialRadius;
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

    // lovely function name
    // this is called by the script that draws the atmospheres (as a screenspace effect) 
    // it gets the indices of the bodies that have atmospheres, and are close enough for those atmos to be rendered
    // IT ALSO SORTS THE INDICES IN ORDER OF DISTANCE TO SAVE WORK ON THE GPU SIDE
    public int[] GetVisibleAtmosphericBodyIndices()
    {
        List<int> visibleBodies = new List<int>();

        List<int> sortedBodies = new List<int>();

        return sortedBodies.ToArray();
    }

    public double GetSeaLevelAltitudeAsDouble()
    {
        //Debug.Log(GetCoreAltitude() + "     " + cb_solarsystem.Instance.monoBodies[GetSOIIndex()].data.tConfig.equitorialRadius);
        return GetCoreAltitudeAsDouble() - (double)cb_solarsystem.Instance.monoBodies[GetSOIIndex()].data.tConfig.equitorialRadius;
    }

    public num_precise GetCoreAltitude()
    {
        return cb_renderingmanager.Instance.player.data.GetPosition().Sub(cb_renderingmanager.Instance.bodyEntities[GetSOIIndex()].data.GetPosition()).Mag();
    }

    public double GetCoreAltitudeAsDouble()
    {
        num_precisevector3 diff = cb_renderingmanager.Instance.player.data.GetPosition().Sub(cb_renderingmanager.Instance.bodyEntities[GetSOIIndex()].data.GetPosition());
        
        return diff.Mag().AsDouble();
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
        ui_playerhud.Instance.SetupDebugInfo();
        UIManager.Instance.HideConsole();

        // -1 means random seed
        if (worldSeed == -1) {worldSeed = util_math.GetRandomInt();}

        this.worldSeed = worldSeed;
        ss.Generate(worldSeed);
        
        CameraController.SetControlMode(CameraControlMode.Freecam);
        UIManager.Instance.EnterMapView();

        Program.gameState = GameState.InGame;

        cb_renderingmanager.Instance.SetupEntities();

        // VERY MUCH TEMP
        LocalPlayer.Instance.SystemTeleport(0);

        onNewWorldGenerate.Invoke();
    }

    // just putting this here for now
    public void SetupMap()
    {
        if (mapFocusIndex == 0) {mapFocusIndex = 1;}
        // for the actual game, there are 2 different lists of objects
        // 'bodies' are the actual planet objects
        // 'map bodies' are specifically for the map view

        if (t_mapBodyContainer.childCount != ss.monoBodies.Count)
        {
            RegenerateMapBodies();
        }

        ui_mapview.Instance.SetupDebugInfo();

        RegenerateMap();
    }

    public void RegenerateMap()
    {
        p = ss.GetBodyPositions(GetMapScaleFromFocusedBody());

        for (int i = 0; i < t_mapBodyContainer.childCount; i++)
        {
            // subtracting the position of the center body focuses on it
            // easier than moving the camera ig?
            t_mapBodyContainer.GetChild(i).GetComponent<cb_mapobject>().Initialize(p[i] - GetMapBasePosition());
        }

        RefreshMap();
    }

    public void RefreshMap()
    {
        for (int i = 0; i < t_mapBodyContainer.childCount; i++)
        {
            // subtracting the position of the center body focuses on it
            // easier than moving the camera ig?
            t_mapBodyContainer.GetChild(i).GetComponent<cb_mapobject>().SetPosition(p[i] - GetMapBasePosition());
        }
        for (int i = 0; i < t_mapBodyContainer.childCount; i++)
        {
            t_mapBodyContainer.GetChild(i).localPosition = p[i] - GetMapBasePosition();
        }
    }

    public void UpdateMapBasePosition()
    {
        if (p == null) {mapBasePosition = Vector3.zero;}

        else
        {
            mapBasePosition =  Vector3.Lerp(mapBasePosition, p[mapFocusIndex], mapLerpSpeed);
        }
    }

    public Vector3 GetMapBasePosition()
    {
        return mapBasePosition;
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
        SetMapFocus(GetNextMapIndex(mapFocusIndex));

        RegenerateMap();
    }

    public void SetMapFocus(int newIndex)
    {
        oldMapFocusIndex = mapFocusIndex;
        mapFocusIndex = newIndex;
        RegenerateMap();
    }

    
    // we switch map bodies like ksp [planet -> moon -> moon -> moon -> planet -> moon]
    // this takes a bit of work, since the planet indices are organized [planet -> planet -> planet -> moon -> moon]

    // the last implementation of this was a shitshow so i fixed it
    public int GetNextMapIndex(int currentIndex)
    {
        int result = currentIndex;

        int planetIndex;
        int moonIndex;

        // checking if planet
        if (cb_solarsystem.Instance.monoBodies[currentIndex].data.pConfig.parentIndex == 0)
        {
            planetIndex = currentIndex;
            moonIndex = -1;
        } else
        {
            moonIndex = currentIndex;
            planetIndex = cb_solarsystem.Instance.monoBodies[currentIndex].data.pConfig.parentIndex;
        }

        if (moonIndex == -1)
        {
            // we're dealing with a planet, and either advance to its next moon or the next planet (if there are no moons)
            
            if (cb_solarsystem.Instance.monoBodies[currentIndex].naturalSatellites.Length > 0)
            {
                // advance to the first moon
                result = cb_solarsystem.Instance.monoBodies[currentIndex].naturalSatellites[0].data.pConfig.selfIndex;
            } else
            {
                // next planet (if there is none it'll get wrapped back to index 1 (the star))
                result = currentIndex + 1;

                // if the index is too high, wrap the result
                if (result > cb_solarsystem.Instance.monoBodies.Count - 1)
                {
                    result = 1;
                }

                if (cb_solarsystem.Instance.monoBodies[result].data.pConfig.parentIndex != 0)
                {
                    // no next planet either, so wrap
                    result = 1;
                }
            }
        } else
        {
            // dealing with a moon, so either advance to the next moon or the next planet

            // seeing if there is a next moon
            bool hasFoundNextMoon = false;
            for (int i = 0; i < cb_solarsystem.Instance.monoBodies.Count; i++)
            {
                if (cb_solarsystem.Instance.monoBodies[i].data.pConfig.parentIndex == planetIndex && i > currentIndex)
                {
                    result = i;
                    hasFoundNextMoon = true;
                    break;
                }
            }

            if (!hasFoundNextMoon)
            {
                // no next moon, so next planet
                result = planetIndex+1;

                // if the index is too high, wrap the result
                if (result > cb_solarsystem.Instance.monoBodies.Count - 1)
                {
                    result = 1;
                }

                if (cb_solarsystem.Instance.monoBodies[result].data.pConfig.parentIndex != 0)
                {
                    // no next planet either, so wrap
                    result = 1;
                }
            }
        }

        // in theory we should never get here
        return result;
    }

    public float GetMapScaleFromFocusedBody()
    {
        float rawScl = mapFocusIndex < 2 ? mapScaleFactorSolar : mapScaleFactorPlanetary;

        return rawScl * WorldData.universalScaleFactor;
    }
}
