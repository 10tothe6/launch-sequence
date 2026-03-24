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
    public ui_console menu;

    public static cmd_consolecommand[] possibleCommands = new cmd_consolecommand[]
    {
        new cmd_consolecommand(new string[]{"teleport","tp"}), // instantly move the player to a coordinate
        new cmd_consolecommand(new string[]{"timeset","t"}), // set the time of the solar system

        new cmd_consolecommand(new string[]{"whitelist","wl"}), // allow a player on a server
        new cmd_consolecommand(new string[]{"blacklist","bl"}), // block a player from a server
        
        new cmd_consolecommand(new string[]{"kick","k","kickplayer"}), // remove a player from a server
        new cmd_consolecommand(new string[]{"ban","b","banplayer"}), // kick + blacklist
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
        if (possibleCommands[0].IsValid(items[0])) // teleport
        {
            // this will post the error message too if it fails
            if (ArgCheck(items, possibleCommands[0]))
            {
                // actually processing the command

                // TODO: the processing logic
            }
        } 
        else if (possibleCommands[1].IsValid(items[0])) // timeset
        {
            
        } 
        else if (possibleCommands[2].IsValid(items[0])) // whitelist
        {
            
        } 
        else if (possibleCommands[3].IsValid(items[0])) // blacklist
        {
            
        } 
        else if (possibleCommands[4].IsValid(items[0])) // kick
        {
            
        } 
        else if (possibleCommands[5].IsValid(items[0])) // ban
        {
            
        } else
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
