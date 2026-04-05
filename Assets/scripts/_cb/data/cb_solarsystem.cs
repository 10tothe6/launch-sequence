using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum cb_bodytype
{
    Null=0,
    Stellar=1, // (stars)
    Terran=2, // (rocky planets)
    Jovian=3, // (gas giants) could also have a 'neptunian' category for ice giants but eh
    TerranMoon=4, // (moons of rocky planets)
    JovianMoon=5, // (moons of gas giants) these are larger than terran ones and have a chance of having atmospheres
}

// TODO: planet names of some kind

public class cb_solarsystem : MonoBehaviour
{
    // i would normally have an issue with this, but we're not adding to this list often
    public static string BodyType(int index)
    {
        if (index == 0) {return "Null";}

        if (index == 1) {return "Stellar";}
        if (index == 2) {return "Terran";}
        if (index == 3) {return "Jovian";}
        if (index == 4) {return "TerranMoon";}
        if (index == 5) {return "JovianMoon";}

        return "[Unknown]";
    }
    private static cb_solarsystem _instance;

    public static cb_solarsystem Instance
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

    // it would be boring if all the planets started out in a line like how they're generated,
    // so after generation we advance time by a random amount
    // this creates two time-frames: 'math-time' and 'game-time'
    
    // this varaiable is the offset that game-time has from math-time
    public float temporalOffset;

    public LootTableEntry[] terranMoonCounts;
    public LootTableEntry[] jovianMoonCounts;

    public GameObject p_body; // and sherman
    public Transform t_bodyContainer;
    public List<cb_trackedbody> monoBodies;

    // proper constants
    public static float gravConstant = 0.1f;

    [Header("System Configs")]

    // constants-ish
    // these I can change quickly as a dev,
    // might move them into some other sort of class later if this script balloons too much
    public int minimumPlanetCount;
    public int maximumPlanetCount;
    private int currentPlanetCount;

    // ******************** full solar system scaling ********************

    // TODO: make this a function of distance
    public float minimumPlanetSpacing;
    public float maximumPlanetSpacing;


    // there is no maximum planet spacing
    public float minimumSystemRadius; // as close to the sun as it gets
    public float maximumSystemRadius; // solar systems can only be so big

    // ****************************************

    // ******************** planetary system scaling ********************

    public float minimumMoonSpacing;
    public float maximumMoonSpacing;

    // how big/small a single planetary system can be
    public float minimumPlanetarySystemRadius;
    public float maximumPlanetarySystemRadius;

    // ****************************************'

    [Header("Basic Body Configs")]

    // terran
    public float minimumTerranSurfaceRadius;
    public float maximumTerranSurfaceRadius;

    public float chanceForTerrainAtmosphere;
    public float minimumTerranAtmosphereRadius;
    public float maximumTerranAtmosphereRadius;

    // jovian
    public float minimumJovianSurfaceRadius; // no "surface" but you get the idea
    public float maximumJovianSurfaceRadius;

    public float minimumJovianAtmosphereRadius;
    public float maximumJovianAtmosphereRadius;

    // terran lunar (these can't have atmospheres, ever)
    public float minimumTerranLunarSurfaceRadius;
    public float maximumTerranLunarSurfaceRadius;

    // jovian lunar (these can't have atmospheres, either, for now)
    public float minimumJovianLunarSurfaceRadius;
    public float maximumJovianLunarSurfaceRadius;

    [Header("Body Atmosphere Configs")]

    // how many TYPES of gasses can appear in each atmosphere
    public int minimumTerranGasCount;
    public int maximumTerranGasCount;

    public int minimumJovianGasCount;
    public int maximumJovianGasCount;


    public float terranExoticGasChance;


    public float jovianThirdExoticGasChance;
    public float jovianFourthExoticGasChance;
    public float jovianFifthExoticGasChance;
    // six is impossible

    [Header("ACTUAL DATA")]
    public cb_solarsystemdata data;

    public int GetPlanetCount()
    {
        return currentPlanetCount;
    }

    public void LoadFromData(cb_solarsystemdata data)
    {
        // first, we make sure to overwrite old data
        this.data = data;
    }

    public void SetRawTimeOffset(float time)
    {
        SetTimeOffset(time - temporalOffset);
    }
    // before, I had one class that would update all of the bodies (TrackingManager)
    // this was back when I was obsessed with ___Manager.cs scripts
    // this time, bc it makes more sense to me, I'm putting the time/position control in here
    // though I may move it later
    public void SetTimeOffset(float time)
    {
        for (int i = 0; i < monoBodies.Count; i++)
        {
            if (i == 0 || i == 1) {monoBodies[i].pose.data.localPosition = num_precisevector3.zero; continue;}
            monoBodies[i].pose.data.localPosition = monoBodies[i].data.pConfig.GetPositionAtTime(time + temporalOffset, 1000);
        }
    }

    public Vector3[] GetBodyPositions(float scaleFactor)
    {
        Vector3[] positions = new Vector3[monoBodies.Count];

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == 0 || i == 1) {positions[i] = Vector3.zero; continue;}
            positions[i] = monoBodies[monoBodies[i].data.pConfig.parentIndex].pose.data.localPosition.ToVector3() +  monoBodies[i].pose.data.localPosition.ToVector3();
            positions[i] *= scaleFactor;
        }

        return positions;
    }

    public Vector3[] GetBodyVelocities(float scaleFactor)
    {
        Vector3[] velocities = new Vector3[monoBodies.Count];

        for (int i = 0; i < velocities.Length; i++)
        {
            if (i == 0 || i == 1) {velocities[i] = Vector3.zero; continue;}
            velocities[i] = monoBodies[monoBodies[i].data.pConfig.parentIndex].pose.data.velocity.ToVector3() +  monoBodies[i].pose.data.velocity.ToVector3();
            velocities[i] *= scaleFactor;
        }

        return velocities;
    }

    public void Generate() {Generate(0);}

    // makes more sense to throw this function inside the class itself... i think
    public void Generate(int worldSeed)
    {   
        // TODO: make the world seed actually work


        // getting rid of any existing body objects
        ui_canvasutils.DestroyChildren(t_bodyContainer.gameObject);
        monoBodies = new List<cb_trackedbody>();

        // quick note - all celestial bodies have orbits
        // including stars
        // they all orbit the "center of mass" of the solar system, which is itself a body
        // this is to allow for multiple stars later

        // type 0 body is a non-visible one
        AddBodyToSystem("COM", -1,(ushort)cb_bodytype.Null,0); // Center Of Mass
        // ^ this will occupy index 0, always

        // we don't need binary star systems rn
        // got too many fuckin problems and this aint gonna be one
        int starCap = 1;

        for (int i = 0; i < starCap; i++)
        {
            // type 1 body is stellar
            // I'll name them later
            AddBodyToSystem("Star", 0, (ushort)cb_bodytype.Stellar,0); // parent them all to COM
        }

        // systems can't have no planets, but they CAN have one
        int planetCap = Random.Range(Instance.minimumPlanetCount,Instance.maximumPlanetCount);

        currentPlanetCount = planetCap;

        float currentRadius = minimumSystemRadius * WorldData.universalScaleFactor - minimumPlanetSpacing * WorldData.universalScaleFactor;

        // keeping track so we can add moons later
        // in theory this is not necessary bc we can just make due with index math,
        // but I don't want to do that
        List<cb_trackedbody> addedPlanets = new List<cb_trackedbody>();

        for (int i = 0; i < planetCap; i++)
        {
            currentRadius += Random.Range(minimumPlanetSpacing * WorldData.universalScaleFactor, maximumPlanetSpacing * WorldData.universalScaleFactor);

            if (currentRadius >= maximumSystemRadius * WorldData.universalScaleFactor)
            {
                currentPlanetCount = i+1;
                // system got too big, regardless of whether we hit our planet cap or not
                break;
            } else
            {
                ushort bodyType = (ushort)cb_bodytype.Terran;

                // the reason we calculate the jovian chance based on distance is because
                // planets further out should be more likely to be gas giants
                float jovianChance = PercentChanceForJovian(currentRadius);

                if (Random.Range(0f, 1000f) < 1000f * jovianChance)
                {
                    bodyType = (ushort)cb_bodytype.Jovian;
                }

                // spawn the planet itself
                addedPlanets.Add(AddBodyToSystem("Planet " + i.ToString(), 0, bodyType,currentRadius)); // again, parent to COM
            }
        }

        // adding moons
        for (int i = 0; i < addedPlanets.Count; i++)
        {
            int moonCap = 0;

            // moons, unlike planets and stars (at least for now) are counted via a loot table
            // this means we can bias things toward no moons, or a median amount of moons
            if (addedPlanets[i].data.bodyType == (ushort)cb_bodytype.Jovian)
            {
                moonCap = int.Parse(LootTableEntry.Get(jovianMoonCounts));
            } else if (addedPlanets[i].data.bodyType == (ushort)cb_bodytype.Terran)
            {
                moonCap = int.Parse(LootTableEntry.Get(terranMoonCounts));
            }
            // not sure how we could hit neither of those, but if we do then no moons

            currentRadius = minimumPlanetarySystemRadius * WorldData.universalScaleFactor - minimumMoonSpacing * WorldData.universalScaleFactor;

            for (int j = 0; j < moonCap; j++)
            {
                // like planets, moons stop either when we reach the cap or the max radius

                currentRadius += Random.Range(minimumMoonSpacing * WorldData.universalScaleFactor, maximumMoonSpacing * WorldData.universalScaleFactor);

                if (currentRadius >= maximumPlanetarySystemRadius * WorldData.universalScaleFactor)
                {
                    break;
                } else
                {
                    ushort bodyType = 
                    addedPlanets[i].data.bodyType == (ushort)cb_bodytype.Terran ? 
                    (ushort)cb_bodytype.TerranMoon : 
                    (ushort)cb_bodytype.JovianMoon;

                    // plus 2 bc of COM and star
                    AddBodyToSystem("Moon " + i.ToString() + "." + j.ToString(), i+2, bodyType,currentRadius);
                }
            }
        }

        temporalOffset = Random.Range(10f, 30f);
        SetTimeOffset(0);

        // making sure every cb knows what its moons are
        for (int i = 0; i < monoBodies.Count; i++)
        {
            monoBodies[i].naturalSatellites = monoBodies[i].GetMoons();
        }
    }

    public bool IntersectBodies(Vector3 rayDir, float dist, int[] avoidBodies)
    {
        bool hit = false;

        for (int i = 0; i < monoBodies.Count; i++)
        {
            Vector2 intersectResult = util_math.RaySphere(monoBodies[i].pose.data.GetPosition().ToVector3(), monoBodies[i].data.tConfig.equitorialRadius, cb_renderingmanager.GetControlPosition(), rayDir);

            if (intersectResult.x < dist && intersectResult.x > -1 && !avoidBodies.Contains(i) && i > 1)
            {
                //Debug.Log(avoidBodies[0] + "     " + i);
                hit = true;
                break;
            }
        }

        return hit;
    }

    public float PercentChanceForJovian(float distanceFromCOM)
    {
        //return 0.5f;
        return 0f;
    }

    // regardless of type (planet, star, etc.)
    // returns the mono so we can do more stuff with it later
    public cb_trackedbody AddBodyToSystem(string name, int parentIndex, ushort bodyType,float baseRadius)
    {
        cb_trackedbody newBody = Instantiate(p_body, t_bodyContainer).GetComponent<cb_trackedbody>();
        // we need to define the 'type' of body so the script knows where to pull data from
        monoBodies.Add(newBody);
        newBody.Initialize(name, parentIndex, bodyType,baseRadius);
        
        return newBody;
    }

    public cb_trackedbody GetBody(string name)
    {
        return data.bodies[GetBodyIndex(name)];
    }

    public int GetBodyIndex(string name)
    {
        for (int i = 0; i < data.bodies.Count; i++)
        {
            if (data.bodies[i].name == name)
            {
                return i;
            }
        }

        return -1;
    }
}
