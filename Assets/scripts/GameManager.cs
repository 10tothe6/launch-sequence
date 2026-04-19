using UnityEngine;
using UnityEngine.SocialPlatforms;

// used so that very high-level scripts like the WorldManagr can only run certain logic when in-game
// essentially the updated version of the inGame variable all the way back from Tempest
public enum GameState
{
    InMenu,
    InGame,
}


// in between Program.cs and everything else

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
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

    // the static one is the one that scripts look for
    // the ins_ variable is just so that I can see
    [Header("vv READ ONLY vv")]
    public GameState ins_gameState;
    public static GameState gameState;

    // into the main menu - this is what player's get
    public static void StartFullGame()
    {
        SwitchToMainMenu(); // this will change the gameState to InMenu
    }

    // straight into the sandbox of a singleplayer server
    public static void StartSingleplayerSandbox()
    {
        gameState = GameState.InGame;

        NetworkHelper.Instance.StartSingleplayerGame(); // starts up the server

        // need to do this so we wait until the server is set up
        ServerNetworkManager.Instance.onJoinServer.AddListener(TrySwitchToSandbox);

        // TODO: moving into the sandbox upon server/game start
    }

    // ditto as above, but for a multiplayer server
    public static void StartMultiplayerSandbox()
    {
        gameState = GameState.InGame;


        NetworkHelper.Instance.StartMultiplayerGame(); // starts up the server

        // need to do this so we wait until the server is set up
        ServerNetworkManager.Instance.onJoinServer.AddListener(TrySwitchToSandbox);


        // TODO: moving into the sandbox  upon server/game start
    }

    // straight into the main game of a singleplayer server
    public static void StartSingleplayerGame()  // done
    {
        gameState = GameState.InGame;

        NetworkHelper.Instance.StartSingleplayerGame(); // starts up the server

        // there is no more 'WorldManager.StartGame(-1)' here, that'll get called upon joining the server
    }

    // straight into the main game of a multiplayer server
    public static void StartMultiplayerGame()  // done
    {
        gameState = GameState.InGame;


        NetworkHelper.Instance.StartMultiplayerGame();
    }

    // ====================================
    // generating a new world
    // ====================================
    public static void InitializeNewGame(int worldSeed)
    {
        ui_playerhud.Instance.SetupDebugInfo();
        UIManager.Instance.HideConsole();

        UIManager.Instance.SwitchMenu("");
        
        WorldManager.Instance.GenerateNewWorld(worldSeed);

        SwitchToGame();
        cb_mainmenucontroller.Instance.Hide();
        // the enabling of the component is done on cam_freecam.cs for now
    }


    // ====================================
    // switching, while already in a world
    // ====================================
    public static void SwitchToMainMenu() // done
    {
        gameState = GameState.InMenu;

        UIManager.Instance.EnterMainMenu();
        cb_mainmenucontroller.Instance.Show();
    }


    // easy shortcut
    public static void TrySwitchToSandbox() {
        ServerNetworkManager.Instance.onJoinServer.RemoveListener(TrySwitchToSandbox);

        ClientNetworkManager.Instance.SendCommandRequest(cmd_console.GetCommandData("sbox"), new string[] {LocalPlayer.localClient.username});
    }

    // once we've actually done the beforehand work
    // and are now actually entering the sandbox
    public static void SwitchToSandbox()
    {
        WorldManager.Instance.SetAllBodiesActive(false);
        Sandbox.Instance.EnterSandbox();
    }

    // entering back into the game
    public static void SwitchToGame()
    {
        WorldManager.Instance.SetAllBodiesActive(true);
        Sandbox.Instance.ExitSandbox();
        gameState = GameState.InGame;
        CameraController.cam_main.GetComponent<cbr_applyatmosphere>().isInGame = true;
    }


    // called from the Program.cs Update() method
    public void UpdateGame()
    {
        EntityManager.Instance.UpdateAllEntityPositions();

        // ====================================
        // updating the world, entities, etc.
        // ====================================

        if (LocalPlayer.IsControllingEntity())
        {
            if (LocalPlayer.localClient.isInSandbox)
            {
                // we're in the sandbox
                Sandbox.Instance.UpdateSandbox();
            } else
            {
                // we're in the main game, and so need to render celestial bodies
                WorldManager.Instance.UpdateWorld();
            }
        } else
        {
            // if we're not controlling an entity, we're not in the actual game
        }

        // ====================================
        // updating UI (no distinction made here between sandbox and game)
        // ====================================

        // we want to try and have an update function in as few scripts as possible
        if (gameState == GameState.InGame)
        {
            UIManager.Instance.InGameUpdate();
        } else if (gameState == GameState.InMenu)
        {
            UIManager.Instance.InMenuUpdate();
        }
    }
}
