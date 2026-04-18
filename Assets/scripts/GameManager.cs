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
        EnterMainMenu(); // this will change the gameState to InMenu
    }

    // straight into the sandbox of a singleplayer server
    public static void StartSingleplayerSandbox()
    {
        gameState = GameState.InGame;


        

        // TODO: moving into the sandbox upon server/game start
    }

    // ditto as above, but for a multiplayer server
    public static void StartMultiplayerSandbox()
    {
        gameState = GameState.InGame;


        

        // TODO: moving into the sandbox  upon server/game start
    }

    // straight into the main game of a singleplayer server
    public static void StartSingleplayerGame()
    {
        gameState = GameState.InGame;

        


        NetworkHelper.Instance.StartSingleplayerGame();
        WorldManager.Instance.StartGame(-1);
    }

    // straight into the main game of a multiplayer server
    public static void StartMultiplayerGame()
    {
        gameState = GameState.InGame;


        
    }

    public static void EnterMainMenu()
    {
        gameState = GameState.InMenu;



        UIManager.Instance.EnterMainMenu();
    }

    public static void EnterSandbox() {
        
    }


    // called from the Program.cs Update() method
    public void UpdateGame()
    {
        // ====================================
        // updating the world, entities, etc.
        // ====================================

        if (LocalPlayer.IsControllingEntity())
        {
            if (LocalPlayer.localClient.isInSandbox)
            {
                // we're in the sandbox
            } else
            {
                // we're in the main game, and so need to render celestial bodies
                
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
