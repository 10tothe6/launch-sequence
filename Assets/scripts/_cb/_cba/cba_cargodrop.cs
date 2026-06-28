using UnityEngine;

// not only for spawning it but also figuring out what cargo it has, etc.

public class cba_cargodrop : MonoBehaviour
{
    // lwk unecessary singleton implementation
    private static cba_cargodrop _instance;

    public static cba_cargodrop Instance
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

    public GameObject g_cargoDrop;

    public void SpawnCargoDrop()
    {
        num_precisevector3 pos = cb_solarsystem.Instance.monoBodies[2].pose.data.GetPosition();

        pos = pos.Add(new num_precisevector3(1d, 0d, 0d).Mul(cb_solarsystem.Instance.monoBodies[2].data.tConfig.equitorialRadius + WorldManager.Instance.GetHeightAtDirection(Vector3.right, 2)));

        g_cargoDrop = EntityManager.Instance.SpawnNewEntity("cargodrop", pos);

        // now for the packages IN the cargo drop
        EntityManager.Instance.SpawnNewEntity("package", pos.Add(num_precisevector3.Right().Mul(2f)));
    }
}
