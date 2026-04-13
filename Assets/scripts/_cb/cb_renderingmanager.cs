using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// could have put this code inside of WorldManager or cb_solarsystem,
// but the former would get way too messy and the latter is specifically for generation/data

// so this script exists now, the equivalent (more or less) of the CBRenderingManager.cs script from earlier


// you can really think of this as (FLOATING ORIGIN + PERSPECTIVE TRICK)
public class cb_renderingmanager : MonoBehaviour
{
    private static cb_renderingmanager _instance;

    public static cb_renderingmanager Instance
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
        worldOffset = new num_precisevector3(0,0,0);
    }

    // ************ NEW VARIABLES ************
    public float originSnapBackRadius; // how far from the origin can the player get before they get "corrected"

    // aka. "hard" culling radius, where mimic and floating objects un-render (apart from celestial objs)
    public float primaryCullingRadius;
    // aka. "soft" culling radius, where fixed entities un-render and mimic objects (including planets) start scaling
    public float secondaryCullingRadius; // how far can objects be before they're squished
    // (secondary < primary)
    // see the entities README for more info!

    public float inflationRadius;

    public Transform t_bodyContainer; // could access from cb_solarsystem, but a shortcut feels better

    // another shortcut
    // this is so that I don't have to use GetComponent<>()
    public e_genericentity[] bodyEntities; 


    // TODO: make this a DoubleVector3?
    public num_precisevector3 worldOffset; // the current offset of the world

    // ************************

    public static num_precisevector3 GetControlPosition()
    {
        if (LocalPlayer.localClient == null)
        {
            return new num_precisevector3(0,0,0);
        }
        else
        {
            if (LocalPlayer.localClient.controllingEntity != null)
            {
                return LocalPlayer.localClient.controllingEntity.data.GetPosition();
            } else
            {
                return new num_precisevector3(0,0,0);
            }
        }
    }

    public void SetupEntities()
    {
        bodyEntities = new e_genericentity[cb_solarsystem.Instance.monoBodies.Count];
        t_bodyContainer = cb_solarsystem.Instance.monoBodies[0].transform.parent;

        for (int i = 0; i < bodyEntities.Length; i++)
        {
            bodyEntities[i] = t_bodyContainer.GetChild(i).GetComponent<e_genericentity>();
        }

        // the entity in control will be set elsewhere
        // as such, EVERYTHING NEEDS NULL CHECKS
    }

    // the periodic function, called by WorldManager.cs
    public void UpdateAllBodyPositions()
    {
        if (LocalPlayer.localClient == null) {return;}
        // we have merged cbp_poseinfo and e_floatingentity to serve the same function,
        // so the position doesn't need to be updated

        if (LocalPlayer.localClient.controllingEntity != null)
        {
            // this is the code that "corrects" the world when you get too far from the origin
            // ofc this doesn't apply if there's no controlling entity
            if (LocalPlayer.localClient.controllingEntity.data.reference.position.magnitude > originSnapBackRadius)
            {
                num_precisevector3 shoveFactor = new num_precisevector3(LocalPlayer.localClient.controllingEntity.data.reference.position).Mul(-1);
                // player is too far from (0, 0, 0) so shove em' back
                worldOffset = worldOffset.Add(shoveFactor);

                // same with the spacecrafts, but this is unused rn
                // for (int i = 0; i < spacecraft.Count; i++)
                // {
                //     spacecraft[i].reference.position += shoveFactor;
                // }

                LocalPlayer.localClient.controllingEntity.data.reference.position = Vector3.zero;
            }

            // player
            //LocalPlayer.localClient.entityInControl.data.Refresh();
        }

        // they do need to be refreshed tho
        for (int i = 0; i < bodyEntities.Length; i++)
        {
            bodyEntities[i].data.floatingData.Refresh();
        }
    }

    // where is (0,0,0) in the unity engine, in game space?
    public num_precisevector3 GetOriginInGameSpace() {
        return worldOffset;
    }

    public bool EntityInsideRenderRadius(e_floatingentity _entity) {
        return true; // TODO: change this
    }

    // ******** helpers *********
    public Vector3 AdjustVector(Vector3 _v, int _id) {
        double rot = 0; // TODO: grab the actual planet data
        return new Vector3(_v.z * Mathf.Sin((float)rot * (Mathf.PI / 180)) + _v.x * Mathf.Cos((float)rot * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos((float)rot * (Mathf.PI / 180)) + _v.x * -Mathf.Sin((float)rot * (Mathf.PI / 180)));
    }
    public Vector3 AdjustVectorReverse(Vector3 _v, int _id) {
        double rot = 0;
        return new Vector3(_v.z * Mathf.Sin((float)-rot * (Mathf.PI / 180)) + _v.x * Mathf.Cos((float)-rot * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos((float)-rot * (Mathf.PI / 180)) + _v.x * -Mathf.Sin((float)-rot * (Mathf.PI / 180)));
    }
    public Vector3 AdjustVector(Vector3 _v, float _amount) {
        return new Vector3(_v.z * Mathf.Sin(_amount * (Mathf.PI / 180)) + _v.x * Mathf.Cos(_amount * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos(_amount * (Mathf.PI / 180)) + _v.x * -Mathf.Sin(_amount * (Mathf.PI / 180)));
    }

    // // what even is this?
    // public List<float> GetAngles() {
    //     float[] toReturn = new float[bodyEntities.Length];
    //     for (int i = 0; i < toReturn.Length; i++) {
    //         toReturn[i] = (float)bodyEntities[i].rotation.y;
    //     }
    //     return toReturn.ToList();
    // }
    // public List<float> GetScaleFactors() {
    //     float[] toReturn = new float[celestialBodies.Length];
    //     for (int i = 0; i < toReturn.Length; i++) {
    //         toReturn[i] = GameUtils.planets[i].transform.localScale.x;
    //     }
    //     return toReturn.ToList();
    // }
    // public List<D> GetPositions() {
    //     Vector4[] toReturn = new Vector3[celestialBodies.Length];
    //     for (int i = 0; i < toReturn.Length; i++) {
    //         toReturn[i] = celestialBodies[i].position.Sub(player.position);
    //     }
    //     return toReturn.ToList();
    // }
}
