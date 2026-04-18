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

    // sandbox is like the 'dev scene' thing from White Knuckle
    // IT IS A PART OF THE GAME
    SandboxSingleplayer, 

    SandboxMultiplayer, 

    InstantGameSingleplayer,
    InstantGameMultiplayer,


    // okay, I DO see a reason to put the sandbox in the game, 
    // but not the planet editor
    // THIS IS STRICTLY A DEV TOOL
    BodyEditor, 
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
                GameManager.StartFullGame(); 
            } 
            
            
            else if (startMode == ProgramStartMode.SandboxSingleplayer)
            {
                GameManager.StartSingleplayerSandbox();
            } 
            

            else if (startMode == ProgramStartMode.SandboxSingleplayer)
            {
                GameManager.StartMultiplayerSandbox();
            } 
            
            
            else if (startMode == ProgramStartMode.InstantGameSingleplayer)
            {   
                GameManager.StartSingleplayerGame();
            } 
            
            
            else if (startMode == ProgramStartMode.InstantGameMultiplayer)
            {   
                GameManager.StartMultiplayerGame();
            } 
            
            
            else if (startMode == ProgramStartMode.BodyEditor)
            {
                // not talking to the game manager for this one, because it's outside the game
                BodyEditor.Instance.SetupEditor();
            }
        } 
        else if (buildMode == ProgramBuildMode.ServerBuild) {/* not really relevant rn*/}
    }

    void Update()
    {
        // the game manager handles the specifics
        GameManager.Instance.UpdateGame();
    }

    // forget exiting to main, just quit the damn program
    public void HardQuit()
    {
        Application.Quit(); // NO OTHER SCRIPT IS ALLOWED TO CALL THIS
    }

    public string GetPreviousVersion()
    {
        // TODO: this function
        return version;
    }
}
