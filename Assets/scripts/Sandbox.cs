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

    private bool isActive;

    // called when the game boots into sandbox mode
    // TODO: allow the user to enter sandbox mode from the console, not just the unity editor
    // ^^ THIS SHOULD ALSO ALLOW FOR MULTIPLAYER SANDBOX-ING
    public void EnterSandbox()
    {
        g_parent.SetActive(true);
        isActive = true;
    }

    public void ExitSandbox()
    {
        g_parent.SetActive(false);
        isActive = false;
    }

    public void UpdateSandbox()
    {
        CameraController.Instance.UpdateCamera();
    }
}
