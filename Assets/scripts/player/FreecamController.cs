using UnityEngine;
using UnityEngine.InputSystem;

public class FreecamController : MonoBehaviour
{
    private e_genericentity entityData;
    void Awake()
    {
        entityData = GetComponent<e_genericentity>();

        entityData.onEnterControl.AddListener(EnterControl);
        entityData.onExitControl.AddListener(ExitControl);
    }

    public bool isActive;
    public LayerMask mask;

    void EnterControl()
    {
        CameraController.SetControlMode(CameraControlMode.Freecam);
        isActive = true;
    }

    void ExitControl()
    {
        isActive = false;
    }

    void Update()
    {
        if (isActive)
        {
            bool interactingWithRobot =false;
            RaycastHit hit;
            
            if (Physics.Raycast(CameraController.t_cam.position, CameraController.t_cam.forward, out hit, 10,mask))
            {
                
                if (hit.collider.transform.parent != null)
                {
                    if (hit.collider.transform.parent.gameObject.GetComponent<PlayerController>() != null)
                    {
                        // this means we're interacting with a robot
                        interactingWithRobot = true;
                    }
                }
            }

            if (interactingWithRobot)
            {
                ui_prompt.Instance.DisplayPrompt("press F to control");

                if (Keyboard.current.fKey.wasPressedThisFrame)
                {
                    ServerNetworkManager.Instance.SetControllingEntity(LocalPlayer.localClient.client_index, hit.collider.transform.parent.GetComponent<e_genericentity>());
                }
            } else
            {
                ui_prompt.Instance.DisplayPrompt("");
            }
        }
    }
}
