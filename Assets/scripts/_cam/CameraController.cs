using UnityEngine;
using UnityEngine.Events;

public enum CameraControlMode
{
    MapView,
    Freecam,
    PlayerFirstPerson,
    ShipFirstPerson,
    ShipThirdPerson,
}

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;

    public static CameraController Instance
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

        t_cam = ins_t_cam;
        cam_main = ins_cam_main;
    }

    // what layers should be rendered for each view
    // (allows us to use the same scene for both)
    public LayerMask normalView;
    public LayerMask mapView;

    public Transform ins_t_cam;
    public static Transform t_cam;

    public static Camera cam_main;
    public Camera ins_cam_main;

    public ushort ins_controlMode;
    public static ushort controlMode;

    public static ushort previousControlMode;

    public Vector3 positionRelativeToControlEntity;

    // to help with transitions
    public UnityEvent onChangeControlMode;

    public Vector3 PositionRelativeToControlEntity()
    {
        return positionRelativeToControlEntity;
    }

    public static void SetToMapView()
    {
        Camera.main.cullingMask = Instance.mapView;
    }

    public static void SetToGameView()
    {
        Camera.main.cullingMask = Instance.normalView;
    }


    public static void SetControlMode(CameraControlMode newMode)
    {
        SetControlMode((ushort)newMode);
    }
    public static void SetControlMode(ushort newMode)
    {
        previousControlMode = controlMode;
        controlMode = newMode;
        Instance.ins_controlMode = controlMode;
        
        Instance.onChangeControlMode.Invoke();
    }
}
