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


    // called when UpdateEntity() is called
    // which is called every time UpdateGame() is called on the GameManager
    public UnityEvent onEntityUpdate;

    void Awake()
    {
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

    // called once every frame by the GameMangager
    // the most important function here, really
    public void UpdateEntity()
    {
        if (!LocalPlayer.IsControllingEntity()) {return;} // TODO: not sure why this is here

        // hide/show based on whether the client is in the sandbox or the main game
        if (LocalPlayer.localClient.isInSandbox)
        {
            gameObject.SetActive(data.index < 0);
        } else
        {
            gameObject.SetActive(data.index >= 0);
        }

        onEntityUpdate.Invoke(); // tell any other scripts, like the physics script
        
        if (data.isPhysicsBased)
        {
            // the physics script has already been notified
        } else
        {
            // the other way - update our transform position based on entity position
            UpdateTransform();
        }


        // lastly, any player controller logic
        if (GetComponent<PlayerController>() != null)
        {
            GetComponent<PlayerController>().UpdatePlayer();
        }
    }


    // only if NOT physics based
    void UpdateTransform()
    {
        // TODO: WHY IS THIS HERE???
        if (!LocalPlayer.IsControllingEntity()) {return;}

        

        if (data.entityType == (ushort)e_entitytype.Fixed)
        {
            // literally just move, no scaling or anything
            transform.position = Coord.GetUnityPosition(this);
        }

        else if (data.entityType == (ushort)e_entitytype.Floating)
        {
            float scaleFactor = float.Parse(data.GetDataEntry("scaleFactor"));
            float defaultScale = float.Parse(data.GetDataEntry("defaultScale"));

            num_precisevector3 pos = data.GetPosition();

            // get the position of the camera
            num_precisevector3 camPosition = LocalPlayer.localClient.controllingEntity.data.GetPosition().Add(CameraController.Instance.PositionRelativeToControlEntity());

            if (camPosition.Sub(pos).Mag().AsDouble() > cb_renderingmanager.Instance.secondaryCullingRadius + 1)
            {
                if (camPosition.Sub(pos).Mag().AsDouble() < cb_renderingmanager.Instance.inflationRadius)
                {
                    // inflate

                    transform.localScale = Vector3.one / scaleFactor * defaultScale;
                    transform.position = pos.Add(Coord.originPosition).ToVector3();
                }
                else
                { // far from planet

                
                transform.localScale = Vector3.one / scaleFactor * defaultScale * (cb_renderingmanager.Instance.secondaryCullingRadius / (float)camPosition.Sub(data.GetPosition()).Mag().AsDouble());
                transform.position = pos.Sub(camPosition).Norm().Mul(cb_renderingmanager.Instance.secondaryCullingRadius).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(LocalPlayer.localClient.controllingEntity.transform.position)).ToVector3();


                }
            }
            else
            {
                transform.localScale = Vector3.one / scaleFactor * defaultScale;
                transform.position = pos.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(LocalPlayer.localClient.controllingEntity.transform.position)).ToVector3();
            }
        }


        // else if (data.entityType == (ushort)e_entitytype.Mimic)
        // {
            
        // }
    }


    // the true index (not client index) of the controlling player
    public int GetControllingPlayerIndex()
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
}
