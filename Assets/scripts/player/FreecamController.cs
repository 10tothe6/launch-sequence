using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreecamController : MonoBehaviour
{
    private player_genericcontroller gComp;
    private e_genericentity entityData;
    void Awake()
    {
        gComp = GetComponent<player_genericcontroller>();
        entityData = GetComponent<e_genericentity>();

        entityData.onEnterControl.AddListener(EnterControl);
        entityData.onExitControl.AddListener(ExitControl);
    }

    public bool isActive;
    public LayerMask mask;

    public float moveSpeed;

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
        if (gComp.mostRecentPacket != null)
        {
            float forward = gComp.mostRecentPacket.forward ? 1f : 0f;
            float backward = gComp.mostRecentPacket.back ? -1f : 0f;

            float left = gComp.mostRecentPacket.left ? -1f : 0f;
            float right = gComp.mostRecentPacket.right ? 1f : 0f;

            float up = 0;
            float down = 0;

            LocalPlayer.Instance.MoveBy(
                (transform.forward * (forward + backward) +
                transform.right * (left + right) + 
                transform.up * (up + down)) * moveSpeed);

            if (gComp.mostRecentPacket.mouseRight)
            {
                transform.Rotate(Vector3.up * -gComp.mostRecentPacket.horizontalMouse + transform.right * gComp.mostRecentPacket.verticalMouse, Space.World);
                entityData.data.SetRotation(transform.rotation);

            }
        }

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
