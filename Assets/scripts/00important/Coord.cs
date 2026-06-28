using UnityEngine;

// short for 'coordinates'

// this script will save us from Multiplayer Hell
// aka. fixing the sync issues we were having
// -10^6, june 28

// only worried about server for now

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

    public static Vector3 GetUnityPosition(e_genericentity entity)
    {
        if (entity.data == null) {return Vector3.zero;}

        
        return GetUnityPosition(entity.data.GetPosition());
    }
    public static Vector3 GetUnityPosition(num_precisevector3 gamePosition)
    {
        num_precisevector3 diff = gamePosition.Sub(originPosition);
        return diff.ToVector3();
    }

    public void SetPositionOfOrigin(num_precisevector3 v)
    {
        // move all of the physics objects so everything doesn't break
        OffsetAllEntities(originPosition.Sub(v));

        originPosition = v;

        cb_renderingmanager.Instance.UpdateAllBodyPositions(); // updating all the planets with the new origin
    }

    // this function COULD be in the EntityManager, but it feels more organized to be here
    // ***
    // this includes both physics and non-physics entities, even though the non-physics ones will offset themselves later
    public void OffsetAllEntities(num_precisevector3 offsetToApply)
    {
        Vector3 v = offsetToApply.ToVector3();
        OffsetAllEntities(v);
    }
    public void OffsetAllEntities(Vector3 v)
    {
        // literally just moving their transform positions
        for (int i = 0; i < EntityManager.Instance.allEntities.Count; i++)
        {
            EntityManager.Instance.allEntities[i].transform.position += v;
        }
    }

    // ***
    // TELEPORTING FUNCTIONS
    #region TP FUNCTIONS
    // THESE SHOULD ONLY BE CALLED ON THE SERVER SIDE, AND AS SUCH FORCE-RETURN IF A CLIENT CALLS THEM
    // ***


    // rides off of the below function
    public void PlanetTeleport(e_genericentity entity, int celestialBodyIndex)
    {
        // see above
        if (!ServerNetworkManager.Instance.isServerActive) {cmd.LogRaw("only the server teleports", Color.lightPink);return;}
        // stars dont count as planets
        if (celestialBodyIndex == 0 || celestialBodyIndex == 1) {cmd.LogRaw("that's not a planet!", Color.lightPink);}

        cb_trackedbody body = cb_solarsystem.Instance.monoBodies[celestialBodyIndex];

         // the extra 3 is just a margin to make sure the player doesn't end up underground
        num_precisevector3 offsetVector = num_precisevector3.Right().Mul(WorldManager.SeaLevelRadius(celestialBodyIndex) + WorldManager.Instance.GetHeightAtDirection(Vector3.right, celestialBodyIndex) + 3f);

        num_precisevector3 desiredPosition = body.pose.data.GetPosition().Add(offsetVector);
        TeleportEntity(desiredPosition, entity);
    }



    // as per the new system:
    // ----------------------------------------
    // for a physics entity
    // 1. set their new position in the backend
    // 2. move the world origin to that position
    // 3. move the transform to the world origin

    // for a non-physics entity
    // 1. set their new position
    // eveything else will follow

    public void TeleportEntity(num_precisevector3 newPosition, e_genericentity entity)
    {
        // see above
        if (!ServerNetworkManager.Instance.isServerActive) {cmd.LogRaw("only the server teleports", Color.lightPink);return;}
        
        if (entity.data.isPhysicsBased)
        {
            entity.data.SetPosition(newPosition);
            SetPositionOfOrigin(newPosition); // this will tell all other physics entities to move
            entity.transform.position = Vector3.zero;
        } else
        {
            entity.data.SetPosition(newPosition);
            // origin stays where it is
            
            // TODO: add a check for moving the origin, which technically could happen if the player is controlling the entity we moved
        }
    }

    #endregion
}
