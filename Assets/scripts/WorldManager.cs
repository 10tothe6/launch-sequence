using UnityEngine;

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

    // for safekeeping, basically
    public int worldSeed;

    public cb_solarsystem ss;

    // generates a new solar system
    public void StartGame(int worldSeed)
    {
        this.worldSeed = worldSeed;
        ss.Generate(worldSeed);
    }
}
