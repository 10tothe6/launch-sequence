using UnityEngine;

// short for 'coordinates'

// this script will save us from Multiplayer Hell
// aka. fixing the sync issues we were having
// -10^6, june 28

public class Coord : MonoBehaviour
{
    private static Coord _instance;

    public static Coord Instance
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

        originPosition = new num_precisevector3(0,0,0);
    }


    // what position in game space is the unity origin?
    public static num_precisevector3 originPosition;

    public void SetPositionOfOrigin(num_precisevector3 v)
    {
        originPosition = v;

        // move all of the physics objects so everything doesn't break

    }

    // ***
    // TELEPORTING FUNCTIONS
    // ***

    public void SystemTeleport()
    {
        
    }

    // as per the new system:

    // for a physics entity
    // 1. set their new position in the backend
    // 2. move the world origin to that position
    // 3. move the transform to the world origin

    // for a non-physics entity
    // 1. set their new position
    // eveything else will follow

    public void TeleportEntity(num_precisevector3 newPosition, e_genericentity entity)
    {
        if (entity.data.isPhysicsBased)
        {
            entity.data.SetPosition(newPosition);
            SetPositionOfOrigin(newPosition); // this will tell all other physics entities to move
            entity.transform.position = Vector3.zero;
        } else
        {
            entity.data.SetPosition(newPosition);
            // origin stays where it is
        }
    }
}
