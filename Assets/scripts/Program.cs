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

    public ProgramBuildMode ins_buildMode;
    public static ProgramBuildMode buildMode;
    public ProgramStartMode ins_startMode;
    public static ProgramStartMode startMode;
}
