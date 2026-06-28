using TMPro;
using UnityEngine;

public class ui_usernamedisplay : MonoBehaviour
{
    public ui_seriesdisplay micVolumeDisplay;
    public TextMeshPro usernameText;

    void Awake()
    {
        // // setting the username when the entity gets controllled
        // transform.parent.GetComponent<e_genericentity>().onEnterControl.AddListener(() =>
        // {
        //     int controllingPlayer = transform.parent.GetComponent<e_genericentity>().GetControllingPlayer();

        //     if (LocalPlayer.localClient != ServerNetworkManager.Instance.connectedClients[controllingPlayer])
        //     {
        //         SetUsername(ServerNetworkManager.Instance.connectedClients[controllingPlayer].username);
        //     } else
        //     {
        //         Hide();
        //     }
        // });

        // transform.parent.GetComponent<e_genericentity>().onExitControl.AddListener(Hide);
    }

    public void Hide()
    {
        micVolumeDisplay.gameObject.SetActive(false);
        usernameText.gameObject.SetActive(false);
    }

    public void SetUsername(string name)
    {
        micVolumeDisplay.gameObject.SetActive(true);
        usernameText.gameObject.SetActive(true);

        usernameText.text = name;
    }

    void Update()
    {
        // always make sure that the display faces towards the camera, while still staying upright
        // keep in mind that the display has its forward vector facing backwards
        Vector3 axis = Vector3.Cross(-transform.forward, CameraController.t_cam.position - transform.position);
        axis = Vector3.Project(axis, transform.up);

        transform.Rotate(axis * Vector3.Angle(CameraController.t_cam.position, transform.position) * 0.99f, Space.World);


        // TODO: make this not periodic
        bool usernameFound = false;
        for (int i = 0; i < ServerNetworkManager.Instance.connectedClients.Count; i++)
        {
            if (ServerNetworkManager.Instance.connectedClients[i].controllingEntity == transform.parent.GetComponent<e_genericentity>())
            {
                SetUsername(ServerNetworkManager.Instance.connectedClients[i].username);
                usernameFound = true;
                break;
            }
        }

        if (!usernameFound || LocalPlayer.localClient.controllingEntity == transform.parent.GetComponent<e_genericentity>())
        {
            Hide();
        }
    }
}
