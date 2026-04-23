using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ui_modularmenuentry
{
    public UnityEvent<string> dataSource;
    public UnityEvent<string> onDataUpdate;

    public ushort displayType; // references prefs_entrydisplaytype
}
