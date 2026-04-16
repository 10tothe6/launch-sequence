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

    // the difference between operator and admin commands
    public bool needsAdmin;

    public cmd_commandarg[] args;

    public cmd_consolecommand() {}
    public cmd_consolecommand(string[] names)
    {
        this.names = names;
        needsAdmin = false;
    }
    public cmd_consolecommand(string[] names, bool needsAdmin)
    {
        this.names = names;
        this.needsAdmin = needsAdmin;
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
        new cmd_consolecommand(new string[]{"tp"},false), // teleport
        new cmd_consolecommand(new string[]{"systp"},false), // (planetary) system teleport

        new cmd_consolecommand(new string[]{"fspeed"},false), // freecam speed

        new cmd_consolecommand(new string[]{"whitelist","wlist"},true), // allow a player on a server
        new cmd_consolecommand(new string[]{"blacklist","blist"},true), // block a player from a server
        new cmd_consolecommand(new string[]{"kick","k"},true), // remove a player from a server
        new cmd_consolecommand(new string[]{"ban","b"},true), // kick + blacklist

        new cmd_consolecommand(new string[]{"spawn"},false), // spawn entity

        new cmd_consolecommand(new string[]{"chat","c"},false), // big text for all players

        new cmd_consolecommand(new string[]{"p","perm"},true), // change permission

        // FUTURE:
        new cmd_consolecommand(new string[]{"timeset","t"},false), // set time 
        new cmd_consolecommand(new string[]{"title"},false), // big text for all players
    };
    
    public static cmd_consolecommand GetCommandData(string name)
    {
        for (int i = 0; i < possibleCommands.Length; i++)
        {
            if (possibleCommands[i].names.Contains(name))
            {
                return possibleCommands[i];
            }
        }

        return null;
    }
    
    public void ProcessMessage(TMP_InputField input)
    {
        ProcessMessage(input.text);
    }

    public void ProcessMessage(string text)
    {
        Debug.Log("Processing console message...");
        
        string[] items = util_string.SplitIntoWords(text);

        // the command type is the first word, hence items[0]
        if (GetCommandData("tp").IsValid(items[0])) // tp
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
        else if (GetCommandData("systp").IsValid(items[0])) // systp
        {
            ServerNetworkManager.Instance.SystemTeleport(LocalPlayer.localClient.controllingEntity, int.Parse(items[1]));
            PostToConsole("[CONSOLE] Teleported to system " + items[1]);
        } 
        else if (GetCommandData("fspeed").IsValid(items[0])) // fspeed
        {
            CameraController.Instance.GetComponent<cam_freecam>().moveSpeed = float.Parse(items[1]);
            PostToConsole("[CONSOLE] Set freecam speed to " + items[1]);
        } 
        
        else if (GetCommandData("whitelist").IsValid(items[0])) // whitelist
        {
            ServerNetworkManager.Instance.WhitelistPlayer(items[1]);
        }
        else if (GetCommandData("blacklist").IsValid(items[0])) // blacklist
        {
            ServerNetworkManager.Instance.BlacklistPlayer(items[1]);
        } else if (GetCommandData("kick").IsValid(items[0])) // kick
        {
            ServerNetworkManager.Instance.KickPlayer(items[1]);
        }
        else if (GetCommandData("ban").IsValid(items[0])) // ban
        {
            ServerNetworkManager.Instance.BanPlayer(items[1]);
        }

        else if (GetCommandData("title").IsValid(items[0])) // title
        {
            // TODO:
        } else if (GetCommandData("chat").IsValid(items[0])) // chatting
        {
            // normally chat messages would be delivered through an in-game ui, not the console
            // the reason I'm making it a console command is for testing (dont wanna make a ui)
            // but this command will be permanent, for debug purposes

            string chatMessage = "";

            if (items.Length > 1) // there has to be more data, otherwise its just 'chat' which is meaningless
            {
                for (int i = 1; i < items.Length; i++)
                {
                    chatMessage += items[i];
                    if (i < items.Length - 1)
                    {
                        chatMessage += "";
                    }
                }

                ClientNetworkManager.Instance.SendChatMessageToServer(chatMessage);
            } else
            {
                cmd.Log("you need to provide a chat message when using the 'chat' command.");
            }
        }

        else if (GetCommandData("spawn").IsValid(items[0])) // spawn
        {
            string entityName = items[1];
            EntityManager.Instance.SpawnNewEntity(entityName, LocalPlayer.localClient.controllingEntity.data.GetPosition());
        }

        else if (GetCommandData("p").IsValid(items[0])) // permission change
        {
            ushort newPermissionLevel;
            if (ushort.TryParse(items[2], out newPermissionLevel))
            {
                ServerNetworkManager.Instance.ChangeClientPermissions(items[1], newPermissionLevel);
            }
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
        PostToConsole(msg);

        // unity console
        Debug.Log(msg);
    }
    public void DebugLog(string msg, Color col)
    {
        // our console
        PostToConsole(msg, col);

        // unity console
        Debug.Log(msg);
    }

    public void PostToConsole(string msg)
    {
        menu.PostMessage(msg);
    }
    public void PostToConsole(string msg, Color col)
    {
        menu.PostMessage(msg, col);
    }
}
