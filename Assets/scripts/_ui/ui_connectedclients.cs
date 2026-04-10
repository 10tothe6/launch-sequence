using UnityEngine;

// takes the connected client list and shows it to the user

public class ui_connectedclients : MonoBehaviour
{
    public ui_list clientList;

    void Update()
    {
        clientList.DisplayList(util_string.AddToArray(ClientNetworkManager.Instance.otherUsernames, ClientNetworkManager.Instance.username));
    }
}
