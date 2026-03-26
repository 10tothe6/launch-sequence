using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// could have put this code inside of WorldManager or cb_solarsystem,
// but the former would get way too messy and the latter is specifically for generation/data

// so this script exists now, the equivalent (more or less) of the CBRenderingManager.cs script from earlier

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
        worldOffset = Vector3.zero;
    }

    // **** NEW VARIABLES ****
    public float originSnapBackRadius; // how far from the origin can the player get before they get "corrected"

    // aka. "hard" culling radius, where mimic and floating objects un-render (apart from celestial objs)
    public float primaryCullingRadius;
    // aka. "soft" culling radius, where fixed entities un-render and mimic objects (including planets) start scaling
    public float secondaryCullingRadius;
    // (secondary < primary)
    // see the entities README for more info!

    public Transform t_bodyContainer; // could access from cb_solarsystem, but a shortcut feels better

    public e_floatingentity[] bodyEntities;

    public float originRadius; // how far from the origin can the player get before they get "corrected"
    public float renderRadius; // how far can objects be before they're squished

    public Vector3 worldOffset; // the current offset of the world

    public e_floatingentity player;
    public e_floatingentity entityInControl;

    public void SetupEntities()
    {
        bodyEntities = new e_floatingentity[cb_solarsystem.Instance.monoBodies.Count];
        t_bodyContainer = cb_solarsystem.Instance.monoBodies[0].transform.parent;

        for (int i = 0; i < bodyEntities.Length; i++)
        {
            bodyEntities[i] = new e_floatingentity(cb_solarsystem.Instance.monoBodies[i].transform.GetChild(0));
            bodyEntities[i].defaultScale = 1;
        }

        // 'player' IS actually a gameObject
        player = new e_floatingentity(GameObject.Find("player").transform);
    }

    public void UpdateAllBodyPositions()
    {
        // update all the planet positions, by grabbing their position from the pConfig
        for (int i = 0; i < bodyEntities.Length; i++)
        {
            bodyEntities[i].position = cb_solarsystem.Instance.monoBodies[i].data.pConfig.GetPosition();
            bodyEntities[i].Refresh();
        }

        // player
        player.Refresh();

        if (entityInControl.reference != null)
        {
            // this is the code that "corrects" the world when you get too far from the origin
            // ofc this doesn't apply if there's no controlling entity
            if (entityInControl.reference.position.magnitude > originRadius)
            {
                Vector3 shoveFactor = -entityInControl.reference.position;
                // player is too far from (0, 0, 0) so shove em' back
                worldOffset += shoveFactor;

                // same with the spacecrafts, but this is unused rn
                // for (int i = 0; i < spacecraft.Count; i++)
                // {
                //     spacecraft[i].reference.position += shoveFactor;
                // }

                player.reference.position += shoveFactor;
            }
        }
    }

    // where is (0,0,0) in the unity engine, in game space?
    public Vector3 GetOriginInGameSpace() {
        return worldOffset;
    }

    public void SwitchToPlayer() {
        entityInControl = player;
    }

    public bool EntityInsideRenderRadius(e_floatingentity _entity) {
        return true; // TODO: change this
    }

    // ******** helpers *********
    public Vector3 AdjustVector(Vector3 _v, int _id) {
        return new Vector3(_v.z * Mathf.Sin((float)bodyEntities[_id].rotation.y * (Mathf.PI / 180)) + _v.x * Mathf.Cos((float)bodyEntities[_id].rotation.y * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos((float)bodyEntities[_id].rotation.y * (Mathf.PI / 180)) + _v.x * -Mathf.Sin((float)bodyEntities[_id].rotation.y * (Mathf.PI / 180)));
    }
    public Vector3 AdjustVectorReverse(Vector3 _v, int _id) {
        return new Vector3(_v.z * Mathf.Sin((float)-bodyEntities[_id].rotation.y * (Mathf.PI / 180)) + _v.x * Mathf.Cos((float)-bodyEntities[_id].rotation.y * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos((float)-bodyEntities[_id].rotation.y * (Mathf.PI / 180)) + _v.x * -Mathf.Sin((float)-bodyEntities[_id].rotation.y * (Mathf.PI / 180)));
    }
    public Vector3 AdjustVector(Vector3 _v, float _amount) {
        return new Vector3(_v.z * Mathf.Sin(_amount * (Mathf.PI / 180)) + _v.x * Mathf.Cos(_amount * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos(_amount * (Mathf.PI / 180)) + _v.x * -Mathf.Sin(_amount * (Mathf.PI / 180)));
    }

    // what even is this?
    public List<float> GetAngles() {
        float[] toReturn = new float[bodyEntities.Length];
        for (int i = 0; i < toReturn.Length; i++) {
            toReturn[i] = (float)bodyEntities[i].rotation.y;
        }
        return toReturn.ToList();
    }
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
