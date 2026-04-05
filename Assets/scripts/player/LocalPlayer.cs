using UnityEngine;
using UnityEngine.InputSystem;

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

    // void FixedUpdate()
    // {
    //     Debug.Log(cb_renderingmanager.GetControlPosition().AsRawString());
    // }

    public e_floatingentity pose;

    public void MoveBy(Vector3 amt)
    {
        pose.data.localPosition = pose.data.localPosition.Add(new num_precisevector3(amt));
    }

    public void SystemTeleport(int index)
    {
        cb_renderingmanager.Instance.player.data.localPosition = cb_solarsystem.Instance.monoBodies[index + 2].pose.data.GetPosition().Add(Vector3.right * WorldManager.SeaLevelRadius(index + 2) * 2);
    }

    // grabs which keys the player is pressing and turns them into this nice, clean, standard format
    public static player_keypresspacket GetKeypressPacket()
    {
        player_keypresspacket result = new player_keypresspacket();

        result.forward = Keyboard.current.wKey.isPressed;
        result.left = Keyboard.current.aKey.isPressed;
        result.back = Keyboard.current.sKey.isPressed;
        result.right = Keyboard.current.dKey.isPressed;

        result.jump = Keyboard.current.spaceKey.isPressed;

        result.horizontalMouse = Input.mouseMovement.x;
        result.verticalMouse = Input.mouseMovement.y;

        return result;
    }
}
