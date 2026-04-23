using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ui_modularmenuentry
{
    public string data;
    public string displayInfo;
    public UnityEvent<string> onDataUpdate;
    public ushort displayType;
    
    public ui_modularmenuentry() {}

    public ui_modularmenuentry(string data, string displayInfo, UnityEvent<string> onDataUpdate, ushort displayType)
    {
        
    }
}
