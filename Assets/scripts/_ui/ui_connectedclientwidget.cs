using UnityEngine.UI;
using UnityEngine;

// some extra UI functionality that doesn't really fit inside the modular ui component system

public class ui_connectedclientwidget : MonoBehaviour
{
    public Image clientIcon;

    public void UpdateIcon()
    {
        int permissionLevel = ServerNetworkManager.GetClientFromUsername(GetComponent<ui_instantiatable>().heldData).permissionLevel;

        clientIcon.sprite = NetworkResources.Instance.permissionLevelIcons[permissionLevel];
    }
}
