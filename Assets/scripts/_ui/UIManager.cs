using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 2nd in command, basically, after Program.cs

// the UIManager script is probably the only thing that's stayed consistent in my projects
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
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
        
        LoadMenuObjects();
    }

    public Transform t_canvas;

    public GameObject g_console;

    public List<string> menuNames;
    public List<int> menuSiblingIndices;

    // for convinience
    public bool isInMapView;

    public ui_connectedclients connectedclients;


    // not just 'update', because i only want to run this sometimes
    public void InGameUpdate()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            connectedclients.Toggle();
        }
        
        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            ToggleConsole();
        }

        // map-related keypress checks
        if (isInMapView)
        {
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                ExitMapView();
            }
        }
        else
        {
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                EnterMapView();
            }
        }
    }

    public void ShowConsole()
    {
        g_console.SetActive(true);
    }

    public void HideConsole()
    {
        g_console.SetActive(false);
    }

    public void ToggleConsole()
    {
        g_console.SetActive(!g_console.activeSelf);
    }

    // some functions, like  this one, build off of the SwitchMenu() function
    public void EnterMapView()
    {
        ui_debugmenu.Instance.SetTabActive("map", true);
        ui_debugmenu.Instance.SetTabActive("game_main", false);

        SwitchMenu("map view");
        WorldManager.Instance.SetupMap();
        CameraController.SetControlMode(CameraControlMode.MapView);

        Debug.Log("Entered map view.");
        isInMapView = true;
    }

    public void ExitMapView()
    {
        ui_debugmenu.Instance.SetTabActive("map", false);
        ui_debugmenu.Instance.SetTabActive("game_main", true);

        CameraController.SetControlMode(CameraController.previousControlMode); // easy way to toggle back to whatever
        Debug.Log("Map view off.");
        isInMapView = false;
    }

    public void LoadMenuObjects()
    {
        for (int i = 0; i < t_canvas.childCount; i++)
        {
            if (t_canvas.GetChild(i).name[0] != '[') {continue;}

            char tag = t_canvas.GetChild(i).name[1];
            if (tag == 'm')
            {
                menuSiblingIndices.Add(i);
                menuNames.Add(t_canvas.GetChild(i).name.Substring(4));
            }
        }
    }

    public void SwitchMenu(string name)
    {
        int index = -1;
        for (int i = 0; i < menuNames.Count; i++)
        {
            t_canvas.GetChild(i).gameObject.SetActive(false);
            if (menuNames[i] == name)
            {
                index = menuSiblingIndices[i];
            }
        }

        if (index == -1) {Debug.Log("Menu name not found!"); return;}

        t_canvas.GetChild(index).gameObject.SetActive(true);
    }
}
