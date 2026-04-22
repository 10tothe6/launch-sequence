using System.Linq;
using Riptide;
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
    public bool canBeRunLocally;

    public cmd_commandarg[] args;

    public cmd_consolecommand() {}
    public cmd_consolecommand(string[] names)
    {
        this.names = names;
        needsAdmin = false;
        canBeRunLocally = false;
    }
    public cmd_consolecommand(string[] names, bool needsAdmin, bool canBeRunLocally)
    {
        this.names = names;
        this.needsAdmin = needsAdmin;
        this.canBeRunLocally = canBeRunLocally;
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
        new cmd_consolecommand(new string[]{"tp"},false,false), // teleport
        new cmd_consolecommand(new string[]{"systp"},false,false), // (planetary) system teleport

        new cmd_consolecommand(new string[]{"fspeed"},false,true), // freecam speed

        new cmd_consolecommand(new string[]{"whitelist","wlist"},true,false), // allow a player on a server
        new cmd_consolecommand(new string[]{"blacklist","blist"},true,false), // block a player from a server
        new cmd_consolecommand(new string[]{"kick","k"},true,false), // remove a player from a server
        new cmd_consolecommand(new string[]{"ban","b"},true,false), // kick + blacklist

        new cmd_consolecommand(new string[]{"spawn"},false,false), // spawn entity

        new cmd_consolecommand(new string[]{"chat","c"},false,false), // big text for all players

        new cmd_consolecommand(new string[]{"p","perm"},true,false), // change permission

        new cmd_consolecommand(new string[]{"sandbox","sbox"},true,false), // go in/out of sandbox

        // for debugging purposes
        new cmd_consolecommand(new string[]{"error","err"},false,true),
        new cmd_consolecommand(new string[]{"exception","exc"},false,true),


        new cmd_consolecommand(new string[]{"kill"},false,false), // killing an entity


        // FUTURE:
        new cmd_consolecommand(new string[]{"timeset","t"},false,false), // set time 
        new cmd_consolecommand(new string[]{"title"},false,false), // big text for all players
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


    // this is called from the UI
    // it will call ProcessMessage(), and then call ShipMessageToServer() if needed
    public void TryRunCommand(TMP_InputField input)
    {
        ProcessMessage(input.text, LocalPlayer.localClient.client_index);
    }
    
    public void ShipMessageToServer(string msg)
    {
        string[] items = util_string.SplitIntoWords(msg);

        string[] args = new string[items.Length - 1];
        for (int i = 1; i < items.Length; i++)
        {
            args[i-1] = items[i];
        }

        ClientSenders.Instance.SendCommandRequest(GetCommandData(items[0]), args);
    }

    public cmd_consolecommand GetSelectedCommand(string commandName)
    {
        for (int i = 0; i < possibleCommands.Length; i++)
        {
            if (possibleCommands[i].names.Contains(commandName))
            {
                return possibleCommands[i];
            }
        }
        return null;
    }


    // ONLY EVER CALLED ON THE SERVER SIDE
    // NEVER ON CLIENT SIDE
    public void ProcessMessage(string text, ushort fromClientIndex)
    {
        string[] items = util_string.SplitIntoWords(text);

        // the VERY FIRST THING WE HAVE TO DO IS CHECK IF THE COMMAND CAN BE LOCAL
        cmd_consolecommand selectedCommand = GetSelectedCommand(items[0]);
        if (selectedCommand == null) {return;}

        if (!selectedCommand.canBeRunLocally && !ServerNetworkManager.Instance.isServerActive)
        {
            // welp, we can't run it locally so we have to ship it
            ShipMessageToServer(text);
        }

        else
        {
            // we CAN run it locally


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

                    //LocalPlayer.Instance.Teleport(new num_precisevector3(x,y,z));
                }
            } 
            else if (GetCommandData("systp").IsValid(items[0])) // systp
            {
                ServerNetworkManager.Instance.SystemTeleport(LocalPlayer.localClient.controllingEntity, int.Parse(items[1]));
                PostToConsole("[CONSOLE] Teleported to system " + items[1]);
            } 
            else if (GetCommandData("fspeed").IsValid(items[0])) // fspeed
            {
                // TODO:
                // CameraController.Instance.GetComponent<cam_freecam>().moveSpeed = float.Parse(items[1]);
                // PostToConsole("[CONSOLE] Set freecam speed to " + items[1]);
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
                            chatMessage += " ";
                        }
                    }

                    ClientSenders.Instance.SendChatMessageToServer(chatMessage);
                } else
                {
                    cmd.Log("you need to provide a chat message when using the 'chat' command.");
                }
            }

            else if (GetCommandData("error").IsValid(items[0]))
            {
                MakeError();
            } else if (GetCommandData("exception").IsValid(items[0]))
            {
                // this command literally just tries to throw an error
                MakeException();
            }


            else if (GetCommandData("kill").IsValid(items[0]))
            {
                int entityIndex;
                if (int.TryParse(items[1], out entityIndex))
                {
                    EntityManager.Instance.RemoveEntity(entityIndex);
                }
            }

            else if (GetCommandData("spawn").IsValid(items[0])) // spawn
            {
                string entityName = items[1];

                net_connectedclient client = ServerNetworkManager.GetClient(fromClientIndex);
                if (items.Length > 2)
                {
                    // the player has given another username to spawn the thing
                    string username = items[2];
                    if (ServerNetworkManager.GetClientFromUsername(username) != null)
                    {
                        client = ServerNetworkManager.GetClientFromUsername(username);
                    }
                    
                }
                
                if (client.isInSandbox)
                {
                    EntityManager.Instance.SpawnNewEntityInSandbox(entityName, client.controllingEntity.data.GetPosition());
                } else
                {
                    EntityManager.Instance.SpawnNewEntity(entityName, client.controllingEntity.data.GetPosition());
                }
            }

            else if (GetCommandData("sbox").IsValid(items[0])) // in/out of sandbox
            {
                net_connectedclient clientInQuestion = ServerNetworkManager.GetClientFromUsername(items[1]);

                if (clientInQuestion != null)
                {
                    clientInQuestion.ToggleSandbox();
                }
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
    }

    public static void MakeError()
    {
        Debug.LogError("TEST ERROR");
    }

    public static void MakeException()
    {
        string[] items = new string[1];

        items[15] = "test"; // this throws an error
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
