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

    public GameObject p_menuCraft;
    public Transform t_craftContainer;

    // ***
    // CRAFT SPAWNING
    // every time you boot up the game, a random craft will be floating in front of the planet
    // (stealing this idea from KSP 2)
    // ***


    // assembles a random craft
    private void SpawnRandomCraft()
    {
        GameObject g_newCraft = Instantiate(p_menuCraft, t_craftContainer);

        g_newCraft.GetComponent<e_craft>().Initialize(rw_craft.ReadCraftDataFromFile(util_file.GetWorkingDirectory() + "menu craft/one.craft"));
        g_newCraft.transform.localScale = Vector3.one * 20;

        g_newCraft.transform.localPosition = Vector3.zero;
    }

    public void Setup()
    {
        CameraController.SetControlMode(CameraControlMode.MainMenu);
        transform.GetChild(0).GetComponent<test_drawmeshbody>().Draw();

        SpawnRandomCraft();
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
