using UnityEngine;

public class Sandbox : MonoBehaviour
{
    private static Sandbox _instance;

    public static Sandbox Instance
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
        g_parent.SetActive(false);
    }

    public GameObject g_parent;
    public Transform t_objectContainer;
    public GameObject p_robot;

    public cam_firstperson fpCam;

    // called when the game boots into sandbox mode
    // TODO: allow the user to enter sandbox mode from the console, not just the unity editor
    public void StartSandbox()
    {
        g_parent.SetActive(true);


        // here we're creating a player object and giving the user control
        // since this game involves switching characters, we have to actually create a new player character and then assign control to it
        GameObject g_playerObj = Instantiate(p_robot, t_objectContainer);

        fpCam.SetControl(g_playerObj);

        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);
    }
}
