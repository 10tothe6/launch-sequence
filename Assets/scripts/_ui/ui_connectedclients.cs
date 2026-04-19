using UnityEngine;

// takes the connected client list and shows it to the user

public class ui_connectedclients : MonoBehaviour
{
    public RectTransform rt_menuParent;
    public ui_list clientList;

    void Start()
    {
        ServerNetworkManager.Instance.onJoinServer.AddListener(ConstructMenu);

        // both of these get the username of the player in question, and can just use that
        ServerNetworkManager.Instance.onPlayerJoin.AddListener(AddPlayerToMenu);
        ServerNetworkManager.Instance.onPlayerLeave.AddListener(RemovePlayerFromMenu);
    }

    void ConstructMenu()
    {
        clientList.SetItems(ServerNetworkManager.Instance.GetClientUsernames());

        // updating the height of the menu
        rt_menuParent.sizeDelta = new Vector2(rt_menuParent.sizeDelta.x, clientList.GetEffectiveHeight());
    }

    void RemovePlayerFromMenu(string username)
    {
        // TODO: make this a part of the list component?
        for (int i = 0; i < clientList.t_listContainer.childCount; i++)
        {
            ui_instantiatable comp = clientList.t_listContainer.GetChild(i).GetComponent<ui_instantiatable>();
            if (comp != null)
            {
                if (comp.heldData == username)
                {
                    Destroy(comp.gameObject);
                    return;
                }
            }
        }
    }

    void AddPlayerToMenu(string username)
    {
        clientList.AddItem(username);
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
