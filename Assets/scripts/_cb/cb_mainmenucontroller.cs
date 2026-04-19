using UnityEngine;

public class cb_mainmenucontroller : MonoBehaviour
{
    private static cb_mainmenucontroller _instance;

    public static cb_mainmenucontroller Instance
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

    public Transform t_body;
    public float rotSpeed;
    public float camRotSpeed;

    public void Setup()
    {
        CameraController.SetControlMode(CameraControlMode.MainMenu);
        transform.GetChild(0).GetComponent<test_drawmeshbody>().Draw();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    void Update()
    {
        t_body.Rotate(Vector3.up * Time.deltaTime * rotSpeed, Space.World);
    }
}
