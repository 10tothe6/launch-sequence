using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private static InteractionManager _instance;

    public static InteractionManager Instance {
        get => _instance;
        private set {
            if (_instance == null) {
                _instance = value;
            }
            else if (_instance != value) {
                Debug.Log("You messed up buddy.");
                Destroy(value);
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public ui_prompt interactionPrompt;

    void Update()
    {
        // showing the prompt on the LOCAL device
        // ***
        InteractableObject3D interactingWith = CheckLocalPlayerForInteractableObject();

        if (interactingWith != null)
        {
            // show the prompt
            interactionPrompt.DisplayPrompt(interactingWith.hoverPrompt);
        } else
        {
            interactionPrompt.DisplayPrompt("");
        }
        // ***

        // now for actual interaction HANDLING, only runs on server
        if (ServerNetworkManager.Instance.isServerActive)
        {
            for (int i = 0; i < ServerNetworkManager.Instance.connectedClients.Count; i++)
            {
                // first we check if they're controlling something
                if (ServerNetworkManager.Instance.connectedClients[i].controllingEntity == null) {continue;}

                // then we check if they have a player_genericcontroller on them
                player_genericcontroller comp = ServerNetworkManager.Instance.connectedClients[i].controllingEntity.GetComponent<player_genericcontroller>();
                if (comp != null)
                {
                    // and we check if they're pressing the interaction button
                    if (comp.mostRecentPacket == null) {continue;}
                    if (comp.mostRecentPacket.up) // the name for the 'e' key
                    {
                        // so they're attempting to interact, now we do the raycast check
                        RaycastHit hit; 

                        Vector3 pos = comp.GetComponent<int_interactionsource>().src.position;
                        Vector3 dir = comp.GetComponent<int_interactionsource>().src.forward;

                        if (Physics.Raycast(pos, dir, out hit))
                        {
                            if (hit.collider.gameObject.GetComponent<InteractableObject3D>() != null)
                            {
                                hit.collider.gameObject.GetComponent<InteractableObject3D>().HandleInteractByObject(comp.gameObject);
                            } else if (hit.collider.gameObject.GetComponent<InteractCollider>() != null)
                            {
                                hit.collider.gameObject.GetComponent<InteractCollider>().parentObject.HandleInteractByObject(comp.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    public InteractableObject3D CheckLocalPlayerForInteractableObject()
    {
        RaycastHit hit;

        Vector3 pos = CameraController.t_cam.position;
        Vector3 dir = CameraController.t_cam.forward;

        if (Physics.Raycast(pos, dir, out hit))
        {
            if (hit.collider.gameObject.GetComponent<InteractableObject3D>() != null)
            {
                return hit.collider.gameObject.GetComponent<InteractableObject3D>();
            } else if (hit.collider.gameObject.GetComponent<InteractCollider>() != null)
            {
                return hit.collider.gameObject.GetComponent<InteractCollider>().parentObject;
            }
        }

        return null;
    }
}
