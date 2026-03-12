using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ui_debugentry
{
    public string title;
    public UnityAction<string> dataSource;

    public ui_debugentry() {}
    public ui_debugentry(string title, UnityAction<string> dataSource)
    {
        this.title = title;
        this.dataSource = dataSource;
    }
}
