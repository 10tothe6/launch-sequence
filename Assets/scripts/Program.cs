using UnityEngine;

// the highest script in the script heirarchy

public enum ProgramBuildMode
{
    HybridBuild,
    ServerBuild,
}

public enum ProgramStartMode
{
    FullGame,
    Sandbox,
    InstantGame,
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

    // should almost be the ONLY use of the start function
    void Start()
    {
        Boot();
    }

    public void Boot()
    {
        if (buildMode == ProgramBuildMode.HybridBuild)
        {
            if (startMode == ProgramStartMode.FullGame)
            {
                UIManager.Instance.SwitchMenu("main menu");
            } else if (startMode == ProgramStartMode.Sandbox)
            {
                
            } else if (startMode == ProgramStartMode.InstantGame)
            {
                WorldManager.Instance.StartGame(-1);
            }
        } 
        else if (buildMode == ProgramBuildMode.ServerBuild) {/* not really relevant rn*/}
    }

    public ProgramBuildMode ins_buildMode;
    public static ProgramBuildMode buildMode;
    public ProgramStartMode ins_startMode;
    public static ProgramStartMode startMode;

    // forget exiting to main, just quit the damn program
    public void HardQuit()
    {
        Application.Quit();
    }
}
