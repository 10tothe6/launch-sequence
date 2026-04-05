using UnityEngine;

// ************
// the highest script in the script heirarchy
// ************



// this is a very, very forward-thinking feature for when I decide to add dedicated servers
// changing this will affect how the build works, avoiding all client code if set to 'ServerBuild'
public enum ProgramBuildMode
{
    HybridBuild,
    ServerBuild,
}

// how the game should boot
// saves me a lot of time that would have been wasted hanging around the main menu
public enum ProgramStartMode
{
    FullGame,
    Sandbox, // sandbox is like the 'dev scene' thing from White Knuckle
    InstantGame,
}

// used so that very high-level scripts like the WorldManagr can only run certain logic when in-game
// essentially the updated version of the inGame variable all the way back from Tempest
public enum GameState
{
    InMenu,
    InGame,
}

public class Program : MonoBehaviour
{
    private static Program _instance;

    public static Program Instance
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

        buildMode = ins_buildMode;
        startMode = ins_startMode;
    }

    public string version;

    public ProgramBuildMode ins_buildMode;
    public static ProgramBuildMode buildMode;
    public ProgramStartMode ins_startMode;
    public static ProgramStartMode startMode;
    // unlike the other two, the static one is the one that scripts look for
    // the ins_ variable is just so that I can see
    [Header("READ ONLY")]
    public GameState ins_gameState;
    public static GameState gameState;

    // should almost be the ONLY use of the start function
    void Start()
    {
        Boot();
    }

    public void Boot()
    {
        Application.targetFrameRate = 60;

        if (buildMode == ProgramBuildMode.HybridBuild)
        {
            if (startMode == ProgramStartMode.FullGame)
            {
                UIManager.Instance.SwitchMenu("main menu");
            } else if (startMode == ProgramStartMode.Sandbox)
            {
                Sandbox.Instance.StartSandbox();
            } else if (startMode == ProgramStartMode.InstantGame)
            {
                WorldManager.Instance.StartGame(-1);
            }
        } 
        else if (buildMode == ProgramBuildMode.ServerBuild) {/* not really relevant rn*/}
    }

    void Update()
    {
        // we want to try and have an update function in as few scripts as possible
        if (gameState == GameState.InGame)
        {
            UIManager.Instance.InGameUpdate();
            WorldManager.Instance.UpdateWorld();
        }
    }

    // forget exiting to main, just quit the damn program
    public void HardQuit()
    {
        Application.Quit();
    }
}
