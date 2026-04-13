using UnityEngine;
using UnityEngine.InputSystem;

// TURNS OUT THIS IS NOT A TEMP CLASS
// it just functions as a sort of shortcut

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

    public static net_connectedclient localClient;

    public void Setup(net_connectedclient client)
    {
        localClient = client;
    }

    public static num_precisevector3 GetPosition()
    {
        // TODO: make the controlling entity a fixed entity?
        if (localClient.controllingEntity != null)
        {
            return localClient.controllingEntity.data.GetPosition();
        } else
        {
            return new num_precisevector3(0,0,0);
        }
    }

    public void MoveBy(Vector3 amt)
    {
        localClient.controllingEntity.data.localPosition = localClient.controllingEntity.data.localPosition.Add(new num_precisevector3(amt));
    }

    public void SystemTeleport(int index)
    {
        if (localClient.controllingEntity == null) {return;}
        localClient.controllingEntity.data.localPosition = cb_solarsystem.Instance.monoBodies[index + 2].pose.data.GetPosition().Add(Vector3.right * WorldManager.SeaLevelRadius(index + 2) * 2);
    }

    public void Teleport(num_precisevector3 pos)
    {
        
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
