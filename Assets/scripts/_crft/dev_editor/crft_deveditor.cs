using UnityEngine;

public class crft_deveditor : MonoBehaviour
{
    private static crft_deveditor _instance;

    public static crft_deveditor Instance
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


    public GameObject p_craft;



    [Header("CONSOLE")]
    public bool spawnNewCraft;
    public e_craft focusedCraft;
    public string partName;
    public bool addPart;




    [Space(14)]
    [Header("SAVING + LOADING")]
    public bool writeCraftToFile;
    public string craftName;
    public bool loadCraftFromFile;
    
    

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // normally don't want to see this
    }

    public void OpenEditor()
    {
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (spawnNewCraft)
        {
            SpawnNewCraft();
            spawnNewCraft = false;
        }

        if (focusedCraft != null)
        {
            if (addPart)
            {
                addPart = false;

                focusedCraft.AddPart(partName);
            }

            if (writeCraftToFile)
            {
                writeCraftToFile = false;
                
                // writing the craft to the 'saved crafts' folder on disk
                rw_craft.WriteCraftDataToFile(focusedCraft.AssembleCraftData(craftName), util_file.GetWorkingDirectory() + "saved craft/" + craftName + ".craft");
            }
        }

        if (loadCraftFromFile)
        {
            loadCraftFromFile = false;

            // (trying to) loading a craft from a file based on the name specified by the player
            crft_craftdata readData = rw_craft.ReadCraftDataFromFile(util_file.GetWorkingDirectory() + "saved craft/" + craftName + ".craft");

            SpawnNewCraft(readData);
        }
    }

    private void SpawnNewCraft(crft_craftdata data = null)
    {
        // normally one would reference the EntityManager, but not here
        GameObject g_newCraft = Instantiate(p_craft, transform);

        g_newCraft.GetComponent<e_craft>().DisablePhysics();
        g_newCraft.transform.localPosition = Vector3.zero;

        focusedCraft = g_newCraft.GetComponent<e_craft>();

        if (data != null)
        {
            g_newCraft.GetComponent<e_craft>().Initialize(data);
        }
    }
}
