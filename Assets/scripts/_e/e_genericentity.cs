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
        data.floatingData.generic = this;
        data.fixedData.generic = this;
        data.mimicData.generic = this;

        data.reference = transform;
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
            gameObject.SetActive(data.index > 0);
        }
        
        if (data.entityType == (ushort)e_entitytype.Fixed)
        {
            data.fixedData.Refresh();
        }
    }
}
