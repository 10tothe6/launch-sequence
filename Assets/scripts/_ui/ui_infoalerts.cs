using UnityEngine;

public enum ui_infoalerttype
{
    Warning,
    Information,
    Error,
}

// iteration 1 of probably many for a modular UI popup system
// some of this is ported from Drivetools

// and yes, I am not a fan of the script name but it'll do for now


// "why do a modular system in the first place instead of hand-crafting popups for different messages?"
// ^^ this is a self-answering question


public class ui_infoalerts : MonoBehaviour
{
    private static ui_infoalerts _instance;

    public static ui_infoalerts Instance
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

    void Start()
    {
        // we want the game to give you an info alert when the game throws an error
        // not everytime, because that would make the game unplayable
        // ******************************
        // THIS SCRIPT IS RESPONSIBLE FOR POSTING THE CONSOLE MESSAGES TOO
        // ******************************

        Application.logMessageReceived += HandleUnityConsoleMessage;
    }

    public ui_list alertList;

    // we can all thank ChatGPT for forking over this syntax
    // TODO: not have it log EVERY SINGLE error, since errors often repeat every frame
    private void HandleUnityConsoleMessage(string logString, string stackTrace, LogType type)
    {
        // the difference between these two is that 'exception' is unity automatically throwing the message
        // and 'error' is ME doing it through Debug.LogError()
        // (I don't use LogError much but it will be important, especially going into indev-1)
        if (type == LogType.Exception)
        {
            ShowFullscreenAlert("unity error: " + logString,Color.red);
            cmd.LogRaw("[ERROR] " + logString,Color.red);
        } else if (type == LogType.Error)
        {
            ShowFullscreenAlert("logged error: " + logString,Color.red);
            cmd.LogRaw("[ERROR] " + logString,Color.red);
        }
    }

    // for now just a raw message, but I may implement some form of syntax later
    public void ShowFullscreenAlert(string message)
    {
        ShowFullscreenAlert(message, Color.white);
    }
    public void ShowFullscreenAlert(string message, Color col)
    {
        alertList.AddItem(message);
        alertList.t_listContainer.GetChild(alertList.t_listContainer.childCount-1).GetComponent<ui_stringdisplay>().SetColor(col);
    }

    public void RemoveMostRecentFullscreenAlert()
    {
        alertList.RemoveMostRecentItem();
    }
}
