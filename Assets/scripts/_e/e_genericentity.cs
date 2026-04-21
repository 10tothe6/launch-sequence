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

    void Awake()
    {
        data.reference = transform;
        
        data.monoComp = this;
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
