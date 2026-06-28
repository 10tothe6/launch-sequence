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
        InteractableObject3D interactingWith = CheckLocalPlayerForInteractableObject();

        if (interactingWith != null)
        {
            // show the prompt
            interactionPrompt.DisplayPrompt(interactingWith.hoverPrompt);
        } else
        {
            interactionPrompt.DisplayPrompt("");
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
