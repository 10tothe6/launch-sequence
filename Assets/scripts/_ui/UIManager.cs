using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using System.Collections;

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
        if (g_transitionScreen != null) {HardSetTransition(false);}
        if (g_advancementsWidget != null) {g_advancementsWidget.SetActive(false);}
        if (g_characterEditor != null) {g_characterEditor.SetActive(false);}
        if (signalScanner != null) {signalScanner.gameObject.SetActive(false);}
        
        LoadMenuObjects();

        actionsToRunOnceFinishedTransition = new List<UnityAction>();

        inventory.SetActive(false);
    }

    public static bool isTyping;

    public void StartTyping()
    {
        isTyping = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void StopTyping()
    {
        isTyping = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public Transform t_canvas;

    public GameObject g_console;

    public List<string> menuNames;
    public List<int> menuSiblingIndices;

    // for convinience
    public bool isInMapView;

    [Space(30)]
    [Header("Component References")]
    public ui_connectedclients connectedclients;
    public ui_bugreporter bugReporter;
    public ui_pausemenu pauseMenu;
    public ui_mainmenu mainMenu;

    public GameObject inventory;


    // TODO: this but via a shader
    public GameObject g_transitionScreen;
    public Image i_transition;
    private float transitionTargetOpacity; 
    public float transitionSpeed;
    private bool currentlyRunningTransition;
    private List<UnityAction> actionsToRunOnceFinishedTransition;


    public GameObject g_introUsernameText;

    public GameObject g_advancementsWidget;
    public GameObject p_advancementPopup;

    public GameObject g_characterEditor;

    // ui for the charger part (electricity)
    public ui_charger charger;

    public ui_scannerwidget signalScanner;


    public ui_canisterwidget canister;

    #region OPEN/CLOSE

    // can only be called if you pass in the proper part data
    // and also a reference to the part
    public void OpenCanisterMenu(crft_resourcecontainer part_reference)
    {
        canister.gameObject.SetActive(true);

        canister.BuildWidget(part_reference);
    }

    public void CloseCanisterMenu()
    {
        canister.gameObject.SetActive(false);
    }




    // really no reason why you would want to open the menu without a reference to the part
    public void OpenChargerMenu(crft_charger part_reference)
    {
        charger.SetPartReference(part_reference);
        charger.gameObject.SetActive(true);
    }
    public void CloseChargerMenu()
    {
        charger.gameObject.SetActive(false);
        // TODO: update the part
    }

    public void ToggleScanner()
    {
        signalScanner.gameObject.SetActive(!signalScanner.gameObject.activeSelf);
    }


    #endregion


    #region LOCKING

    public void LockPlayer()
    {
        // freeze player movement and looking
        PlayerController comp = LocalPlayer.localClient.controllingEntity.GetComponent<PlayerController>();

        comp.lockCameraHorizontal = true;
        comp.lockCameraVertical = true;
        comp.lockMovement = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void UnlockPlayer()
    {
        // freeze player movement and looking
        PlayerController comp = LocalPlayer.localClient.controllingEntity.GetComponent<PlayerController>();

        comp.lockCameraHorizontal = false;
        comp.lockCameraVertical = false;
        comp.lockMovement = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    # endregion

    public void ToggleCharacterEditor()
    {
        if (g_characterEditor.activeSelf)
        {
            CloseCharacterEditor();
        } else
        {
            OpenCharacterEditor();
        }
    }

    public void OpenCharacterEditor()
    {
        g_characterEditor.SetActive(true);

        // change the player's camera
        CameraController.SetControlMode(CameraControlMode.CharacterEditor);

        OpenInventory();
        ui_inventories.Instance.OpenPlayerInventory();

        g_characterEditor.GetComponent<ui_charactereditor>().isActive = true;
    }
    public void CloseCharacterEditor()
    {
        g_characterEditor.GetComponent<ui_charactereditor>().isActive = false;

        g_characterEditor.SetActive(false);
        UIManager.Instance.CloseInventory();

        CameraController.SetControlMode(CameraControlMode.PlayerFirstPerson);
    }


    #region ADVANCEMENTS


    public IEnumerator ShowAdvancementPopup(adv_advancementdata data)
    {
        Transform t_popup = Instantiate(p_advancementPopup, t_canvas).transform;

        t_popup.position = Vector3.zero + Vector3.up * t_popup.GetComponent<ui_instantiatable>().effectiveHeight;

        yield return new WaitForSeconds(2f);

        Destroy(t_popup.gameObject);
    }


    // these two are so that I have a space to run any additional logic needed
    public void OpenAdvancementsWidget()
    {
        g_advancementsWidget.SetActive(true);
        g_advancementsWidget.GetComponent<ui_advancementwidget>().RenderAchievements();
    }
    public void CloseAdvancementsWidget()
    {
        g_advancementsWidget.SetActive(false);
    }




    public void ToggleAdvancementsWidget()
    {
        if (g_advancementsWidget.activeSelf)
        {
            CloseAdvancementsWidget();
        } else
        {
            OpenAdvancementsWidget();
        }
    }


    #endregion


    public IEnumerator RunIntro()
    {
        HardSetTransition(true);
        g_introUsernameText.SetActive(false);

        yield return new WaitForSeconds(1f);

        // show the '10tothe6' username
        g_introUsernameText.SetActive(true);


        yield return new WaitForSeconds(2f);


        g_introUsernameText.SetActive(false);
        // go to the main menu
        GameManager.SwitchToMainMenu(); // this will change the gameState to InMenu
    }




    // ***
    // transition functions
    // ***


    public void FadeInTransition()
    {
        // making sure its actually off
        HardSetTransition(false);

        transitionTargetOpacity = 1;
        currentlyRunningTransition = true;
    }
    public void FadeInTransitionAndThen(UnityAction toRunOnceFinished)
    {
        FadeInTransition();
        actionsToRunOnceFinishedTransition.Add(toRunOnceFinished);
    }

    public void FadeOutTransition()
    {
        HardSetTransition(true);

        transitionTargetOpacity = 0;

        currentlyRunningTransition = true;
    }
    public void FadeOutTransitionAndThen(UnityAction toRunOnceFinished)
    {
        FadeOutTransition();
        actionsToRunOnceFinishedTransition.Add(toRunOnceFinished);
    }

    public void HardSetTransition(bool shouldBeActive)
    {
        g_transitionScreen.SetActive(shouldBeActive);
        i_transition.gameObject.SetActive(true);
    }

    private void RunAllPostTransitionActions()
    {
        for (int i = 0; i < actionsToRunOnceFinishedTransition.Count; i++)
        {
            actionsToRunOnceFinishedTransition[i].Invoke();
        }

        actionsToRunOnceFinishedTransition.Clear();
    }

    private void UpdateTransition()
    {
        if (currentlyRunningTransition)
        {
            Color c = i_transition.color;
            i_transition.color = new Color(c.r, c.g, c.b, c.a + Mathf.Clamp(transitionTargetOpacity - c.a, -transitionSpeed, transitionSpeed));
            if (i_transition.color.a <= 0)
            {
                i_transition.gameObject.SetActive(false);
            }



            // note - I hate how these two are separate if statements
            if (i_transition.color.a <= 0.75f || i_transition.color.a >= 1)
            {
                RunAllPostTransitionActions();
            }
            if (i_transition.color.a <= 0f || i_transition.color.a >= 1)
            {
                currentlyRunningTransition = false;
            }
        }
    }

    // ***

    public void SetBugReporterActive(bool active)
    {
        bugReporter.gameObject.SetActive(active);
    }
    
    public void TogglePauseMenu()
    {
        if (pauseMenu.gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
        } else {Cursor.lockState = CursorLockMode.None;}




        pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
    }


    public void ToggleInventory()
    {
        if (inventory.gameObject.activeSelf)
        {
            CloseInventory();
        } else
        {
            OpenInventory();
            ui_inventories.Instance.OpenPlayerInventory();
        }
    }
    public void OpenInventory()
    {
        inventory.gameObject.SetActive(true);

        LockPlayer();
    }
    public void CloseInventory()
    {
        ui_inventories.Instance.ClearMenus();
        

        inventory.gameObject.SetActive(false);

        UnlockPlayer();
    }

    public void EnterMainMenu()
    {
        SwitchMenu("main menu");
        mainMenu.LoadMainMenu();
        g_console.SetActive(false);

        inventory.SetActive(false);

        cb_mainmenucontroller.Instance.Setup();
    }

    public void EnterConnectionMenu()
    {
        SwitchMenu("join server menu");
        g_console.SetActive(false);
    }

    public void InMenuUpdate()
    {
        UpdateTransition();
        CameraController.Instance.UpdateCamera();
    }


    // not just 'update', because i only want to run this sometimes
    public void InGameUpdate()
    {
        UpdateTransition();
        CameraController.Instance.UpdateCamera();

        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            ToggleConsole();
        }

        if (!isTyping)
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                TogglePauseMenu();
            }

            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                connectedclients.Toggle();
            }

            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                ToggleInventory();
            }

            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                // toggle build menu
            }

            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
               // enter/exit the character editor
               ToggleCharacterEditor();
            }

            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                ToggleScanner();
            }

            // map-related keypress checks
            if (isInMapView)
            {
                if (Keyboard.current.mKey.wasPressedThisFrame)
                {
                    ExitMapView();
                }
            }
            else if (LocalPlayer.IsControllingEntity())
            {
                if (Keyboard.current.mKey.wasPressedThisFrame && !LocalPlayer.localClient.isInSandbox)
                {
                    EnterMapView();
                }
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
        if (g_console.activeSelf)
        {
            StartTyping();
        } else
        {
            StopTyping();
        }
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
            t_canvas.GetChild(menuSiblingIndices[i]).gameObject.SetActive(false);
            if (menuNames[i] == name)
            {
                index = menuSiblingIndices[i];
            }
        }

        if (index == -1) {Debug.Log("Menu name not found!"); return;}

        t_canvas.GetChild(index).gameObject.SetActive(true);
    }
}
