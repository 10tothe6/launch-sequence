using UnityEngine;

// takes the connected client list and shows it to the user

public class ui_connectedclients : MonoBehaviour
{
    public RectTransform rt_menuParent;
    public ui_list clientList;

    void Start()
    {
        ServerNetworkManager.Instance.onJoinServer.AddListener(ConstructMenu);
    }

    void ConstructMenu()
    {
        clientList.DisplayList(util_string.AddToArray(ServerNetworkManager.Instance.GetClientUsernames(), ClientNetworkManager.Instance.username));
    }

    void UpdateMenu()
    {
        clientList.DisplayList(util_string.AddToArray(ServerNetworkManager.Instance.GetClientUsernames(), ClientNetworkManager.Instance.username));
        
        // updating the height of the menu
        rt_menuParent.sizeDelta = new Vector2(rt_menuParent.sizeDelta.x, clientList.GetEffectiveHeight());
    }

    public void Toggle()
    {
        SetActive(!rt_menuParent.gameObject.activeSelf);
    }
    public void SetActive(bool active)
    {
        rt_menuParent.gameObject.SetActive(active);
    }
}
