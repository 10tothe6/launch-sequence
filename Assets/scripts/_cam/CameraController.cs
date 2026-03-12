using UnityEngine;

public enum CameraControlMode
{
    MapView,
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
    }

    public Transform ins_t_cam;
    public static Transform t_cam;

    public ushort ins_controlMode;
    public static ushort controlMode;


    public static void SetControlMode(CameraControlMode newMode)
    {
        SetControlMode((ushort)newMode);
    }
    public static void SetControlMode(ushort newMode)
    {
        controlMode = newMode;
        Instance.ins_controlMode = controlMode;
    }
}
