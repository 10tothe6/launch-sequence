using TMPro;
using UnityEngine;
using UnityEngine.Events;

// hopefully this doesn't exist already
public enum e_entitytype
{
    Fixed,
    Floating,
    Mimic,
}

// data that goes across all entity types
public class e_genericentity : MonoBehaviour
{
    // basic data for the entity
    public e_genericentitydata data;

    public UnityEvent onEnterControl;
    public UnityEvent onExitControl;
    public bool showEntityId;

    void Awake()
    {
        data.reference = transform;
        
        data.monoComp = this;
    }

    void Start()
    {
        if (showEntityId)
        {
            // spawning the debug text on the entity
            GameObject g_debugText = Instantiate(EntityManager.Instance.p_debugText, transform); // this'll just end up going to the bottom of the child list
            g_debugText.transform.localPosition = Vector3.zero;
            g_debugText.GetComponent<TextMeshPro>().text = data.index.ToString();
        }
    }

    public int GetControllingPlayer()
    {
        for (int i = 0; i < ServerNetworkManager.Instance.connectedClients.Count; i++)
        {
            if (ServerNetworkManager.Instance.connectedClients[i].controllingEntity == this)
            {
                return i;
            }
        }


        return -1;
    }

    public void Refresh()
    {
        if (!LocalPlayer.IsControllingEntity()) {return;}
        // don't do this if the entity is being controlled
        if (LocalPlayer.localClient.controllingEntity == this) {return;}

        // hide/show based on whether the client is in the sandbox or the main game
        if (LocalPlayer.localClient.isInSandbox)
        {
            gameObject.SetActive(data.index < 0);
        } else
        {
            gameObject.SetActive(data.index >= 0);
        }

        data.RefreshRenderedPosition();
    }
}
