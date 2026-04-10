using UnityEngine;

// for now, just a bunch of shortcut functions

public class cmd
{
    public static void Log(string msg)
    {
        cmd_console.Instance.DebugLog(msg);
    }
}
