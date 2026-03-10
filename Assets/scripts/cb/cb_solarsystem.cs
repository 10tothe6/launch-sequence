using UnityEngine;

public class cb_solarsystem : MonoBehaviour
{
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

    // proper constants
    public static float gravConstant = 0.1f;

    // constants-ish
    // these I can change quickly as a dev,
    // might move them into some other sort of class later if this script balloons too much
    public int minimumPlanetCount;
    public int maximumPlanetCount;

    public float minimumPlanetSpacing;
    // there is no maximum planet spacing
    public float maximumSystemRadius; // solar systems can only be so big

    public cb_solarsystemdata data;

    // makes more sense to throw this function inside the class itself... i think
    public static void Generate()
    {
        // systems can't have no planets, but they CAN have one
        int planetCap = Random.Range(Instance.minimumPlanetCount,Instance.maximumPlanetCount);
    }
}
