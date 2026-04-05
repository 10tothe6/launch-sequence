using UnityEngine;

// might end up being a temporary class (by the time we add multiplayer), 
// but we're keeping it for now to help with inputs and stuff

public class LocalPlayer : MonoBehaviour
{
    private static LocalPlayer _instance;

    public static LocalPlayer Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else if (_instance != value)
            {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        Debug.Log(cb_renderingmanager.GetControlPosition().AsRawString());
    }

    public e_floatingentity pose;

    public void MoveBy(Vector3 amt)
    {
        pose.data.localPosition = pose.data.localPosition.Add(new num_precisevector3(amt));
    }

    public void SystemTeleport(int index)
    {
        cb_renderingmanager.Instance.player.data.localPosition = cb_solarsystem.Instance.monoBodies[index + 2].pose.data.GetPosition().Add(Vector3.right * WorldManager.SeaLevelRadius(index + 2) * 2);
    }
}
