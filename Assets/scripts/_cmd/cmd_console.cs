using System.Linq;
using TMPro;
using UnityEngine;

public enum cmd_consoleerror
{
    WrongNumArgs,
}

public enum cmd_commandarg
{
    DecimalNumber,
    IntegerNumber,
    Text,
    PlayerName,
}

[System.Serializable]
public class cmd_consolecommand
{
    // we have multiple so that commands like 'teleport'
    // can also have shorthands ('tp')
    public string[] names;

    public cmd_commandarg[] args;

    public cmd_consolecommand() {}
    public cmd_consolecommand(string[] names)
    {
        this.names = names;
    }

    public bool IsValid(string name)
    {
        return names.Contains(name);
    }
}

public class cmd_console : MonoBehaviour
{
    private static cmd_console _instance;
    public static cmd_console Instance
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

    public ui_console menu;

    public static cmd_consolecommand[] possibleCommands = new cmd_consolecommand[]
    {
        // CURRENT:
        new cmd_consolecommand(new string[]{"tp"}), // teleport
        new cmd_consolecommand(new string[]{"systp"}), // (planetary) system teleport

        new cmd_consolecommand(new string[]{"fspeed"}), // freecam speed

        new cmd_consolecommand(new string[]{"whitelist","wlist"}), // allow a player on a server
        new cmd_consolecommand(new string[]{"blacklist","blist"}), // block a player from a server
        new cmd_consolecommand(new string[]{"kick","k"}), // remove a player from a server
        new cmd_consolecommand(new string[]{"ban","b"}), // kick + blacklist

        new cmd_consolecommand(new string[]{"spawn"}), // spawn entity

        new cmd_consolecommand(new string[]{"title"}), // big text for all players

        new cmd_consolecommand(new string[]{"p"}), // change permission

        // FUTURE:
        new cmd_consolecommand(new string[]{"timeset","t"}), // set time 
    };
    
    public void ProcessMessage(TMP_InputField input)
    {
        ProcessMessage(input.text);
    }

    public void ProcessMessage(string text)
    {
        Debug.Log("Processing console message...");
        
        string[] items = util_string.SplitIntoWords(text);

        // the command type is the first word, hence items[0]
        if (possibleCommands[0].IsValid(items[0])) // tp
        {
            // this will post the error message too if it fails
            if (ArgCheck(items, possibleCommands[0]))
            {
                // actually processing the command

                double x = double.Parse(items[1]);
                double y = double.Parse(items[2]);
                double z = double.Parse(items[3]);

                LocalPlayer.Instance.Teleport(new num_precisevector3(x,y,z));
            }
        } 
        else if (possibleCommands[1].IsValid(items[0])) // systp
        {
            LocalPlayer.Instance.SystemTeleport(int.Parse(items[1]));
            PostToConsole("[CONSOLE] Teleported to system " + items[1]);
        } 
        else if (possibleCommands[2].IsValid(items[0])) // fspeed
        {
            CameraController.Instance.GetComponent<cam_freecam>().moveSpeed = float.Parse(items[1]);
            PostToConsole("[CONSOLE] Set freecam speed to " + items[1]);
        } 
        
        else if (possibleCommands[2].IsValid(items[0])) // whitelist
        {
            ServerNetworkManager.Instance.WhitelistPlayer(items[1]);
        }
        else if (possibleCommands[2].IsValid(items[0])) // blacklist
        {
            ServerNetworkManager.Instance.BlacklistPlayer(items[1]);
        } else if (possibleCommands[2].IsValid(items[0])) // kick
        {
            ServerNetworkManager.Instance.KickPlayer(items[1]);
        }
        else if (possibleCommands[2].IsValid(items[0])) // ban
        {
            ServerNetworkManager.Instance.BanPlayer(items[1]);
        }

        else if (possibleCommands[2].IsValid(items[0])) // title
        {
            CameraController.Instance.GetComponent<cam_freecam>().moveSpeed = float.Parse(items[1]);
            PostToConsole("[CONSOLE] Set freecam speed to " + items[1]);
        }

        else if (possibleCommands[2].IsValid(items[0])) // spawn
        {
            CameraController.Instance.GetComponent<cam_freecam>().moveSpeed = float.Parse(items[1]);
            PostToConsole("[CONSOLE] Set freecam speed to " + items[1]);
        }
        
        
        else
        {
            PostToConsole(items[0]);
        }
    }



    bool ArgCheck(string[] items, cmd_consolecommand command)
    {
        bool validCount = items.Length == command.args.Length + 1;
        if (!validCount)
        {
            PostErrorToConsole(cmd_consoleerror.WrongNumArgs);
        }

        bool validTypes = true;

        return validCount && validTypes;
    }

    public void PostErrorToConsole(cmd_consoleerror error)
    {
        if (error == cmd_consoleerror.WrongNumArgs)
        {
            PostToConsole("Invalid number of arguments!");
        }
    }

    // replaces "debug.log", making it so messages appear in the in-game console as well as the unity one
    public void DebugLog(string msg)
    {
        // our console
        PostToConsole("[DEBUG] " + msg);

        // unity console
        Debug.Log(msg);
    }

    public void PostToConsole(string msg)
    {
        menu.PostMessage(msg);
    }
}
