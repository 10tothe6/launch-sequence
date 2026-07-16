using UnityEngine;
using UnityEngine.Events;

public enum CameraControlMode
{
    None,
    MapView,
    Freecam,
    PlayerFirstPerson,
    BodyEditor,
    MainMenu,
    CharacterEditor,
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

        cam_main.GetComponent<cbr_applyatmosphere>().enabled = false;
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

    public num_precisevector3 positionRelativeToControlEntity;

    // to help with transitions
    public UnityEvent onChangeControlMode;
    public UnityEvent onCameraUpdate;



    // fov-related stuff
    // needs a bit of sysarch to make sure any sub-cameras obey the rules
    private float target_fov;
    [SerializeField]
    private float fov_lerp_speed;

    public static void SetCameraFov(float target_fov, bool should_lerp = true)
    {
        Instance.target_fov = target_fov;

        if (!should_lerp)
        {
            // just set it immediately
            cam_main.fieldOfView = target_fov;
        }
    }

    public void UpdateCamera()
    { 
        onCameraUpdate.Invoke();

        // fov interpolation
        cam_main.fieldOfView = Mathf.Lerp(cam_main.fieldOfView, target_fov, fov_lerp_speed);
    }

    public num_precisevector3 PositionRelativeToControlEntity()
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

    public static void ZeroOut()
    {
        Instance.transform.localPosition = Vector3.zero;
        t_cam.localPosition = Vector3.zero;

        Instance.transform.rotation =Quaternion.identity;
        t_cam.transform.rotation =Quaternion.identity;
    }
    public static void ZeroOutLocal()
    {
        Instance.transform.localPosition = Vector3.zero;
        t_cam.localPosition = Vector3.zero;

        Instance.transform.localRotation =Quaternion.identity;
        t_cam.transform.localRotation =Quaternion.identity;
    }


    public static void SetControlMode(CameraControlMode newMode)
    {
        SetControlMode((ushort)newMode);
    }
    public static void SetControlMode(ushort newMode)
    {
        if (newMode == 0) {Instance.transform.SetParent(null);}
        
        previousControlMode = controlMode;
        controlMode = newMode;
        Instance.ins_controlMode = controlMode;
        
        Instance.onChangeControlMode.Invoke();
    }
}
