using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class cbp_renderingmanager : MonoBehaviour
{
    private static cbp_renderingmanager _instance;

    public static cbp_renderingmanager Instance
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

    public Camera cam;
    public cbp_floatingentity[] bodyEntities;

    // ************************************

    public LayerMask normalView;
    public LayerMask mapView;
    public bool isMapActive;

    public float planetRotSpeed;
    public float originRadius;
    public float renderRadius;
    
    public cbp_floatingentity player;
    //public List<cbr_fixedentity> structures;
    public Vector3 worldOffset;
    public cbp_floatingentity entityInControl;

    void Awake()
    {
        Instance = this;

        worldOffset = Vector3.zero;
    }

    void Start()
    {
        cam.cullingMask = normalView;
    }

    public void InitializeSystem()
    {
        bodyEntities = new cbp_floatingentity[cb_solarsystem.Instance.monoBodies.Count];
        for (int i = 0; i < bodyEntities.Length; i++)
        {
            bodyEntities[i] = new cbp_floatingentity(cb_solarsystem.Instance.monoBodies[i].transform);
            bodyEntities[i].defaultScale = 1;
        }

        player = new cbp_floatingentity(GameObject.Find("player").transform);
    }

    public void UpdateAll()
    {
        // planets
        for (int i = 0; i < bodyEntities.Length; i++)
        {
            bodyEntities[i].position = cb_solarsystem.Instance.monoBodies[i].data.pConfig.GetPosition();
            bodyEntities[i].Refresh();
        }
        // player
        player.Refresh();

        if (entityInControl.reference != null)
        {
            if (entityInControl.reference.position.magnitude > originRadius)
            {
                Vector3 shoveFactor = -entityInControl.reference.position;
                // player is too far from (0, 0, 0) so shove em' back
                worldOffset += shoveFactor;
                // for (int i = 0; i < spacecraft.Count; i++)
                // {
                //     spacecraft[i].reference.position += shoveFactor;
                // }
                player.reference.position += shoveFactor;
            }
        }
    }

    // returns the offset of the camera relative to the entity in control
    public Vector3 GetCameraOffset() {
        return CameraController.Instance.PositionRelativeToControlEntity();
    }

    // // Add a ship to the list (when a ship is created)
    // public void AddShip(Transform _toAdd) {
    //     spacecraft.Add(new cbr_floatingentity(_toAdd));
    // }

    // public void AddShip(Transform _toAdd, Vector3 _pos, Vector3 _rot) {
    //     spacecraft.Add(new cbr_floatingentity(_toAdd, _pos, _rot));
    // }

    // // Add a structure to the list (when a structure is generated)
    // public void AddStructure(Transform _toAdd, int _body) {
    //     structures.Add(new cbr_fixedentity(_toAdd, _body));
    // }

    public bool EntityInsideRenderRadius(cbp_floatingentity _entity) {
        return true; // change this
    }

    public Vector3 GetOriginInGameSpace() {
        return worldOffset;
    }

    // // Functions for switching between entities
    // public void SwitchToShip(int _shipId) {
    //     entityInControl = spacecraft[_shipId];
    // }

    // public void SwitchToShip(Transform _shipTransform) {
    //     // find the entity that matches the transform
    //     int foundShip = -1;
    //     for (int i = 0; i < spacecraft.Count; i++) {
    //         if (spacecraft[i].reference == _shipTransform) {
    //             foundShip = i;
    //             break;
    //         }
    //     }

    //     entityInControl = spacecraft[foundShip];
    // }

    public void SwitchToPlayer() {
        entityInControl = player;
    }

    public Vector3 AdjustVector(Vector3 _v, int _id) {
        return new Vector3(_v.z * Mathf.Sin((float)bodyEntities[_id].rotation.y * (Mathf.PI / 180)) + _v.x * Mathf.Cos((float)bodyEntities[_id].rotation.y * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos((float)bodyEntities[_id].rotation.y * (Mathf.PI / 180)) + _v.x * -Mathf.Sin((float)bodyEntities[_id].rotation.y * (Mathf.PI / 180)));
    }

    public Vector3 AdjustVectorReverse(Vector3 _v, int _id) {
        return new Vector3(_v.z * Mathf.Sin((float)-bodyEntities[_id].rotation.y * (Mathf.PI / 180)) + _v.x * Mathf.Cos((float)-bodyEntities[_id].rotation.y * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos((float)-bodyEntities[_id].rotation.y * (Mathf.PI / 180)) + _v.x * -Mathf.Sin((float)-bodyEntities[_id].rotation.y * (Mathf.PI / 180)));
    }

    public Vector3 AdjustVector(Vector3 _v, float _amount) {
        return new Vector3(_v.z * Mathf.Sin(_amount * (Mathf.PI / 180)) + _v.x * Mathf.Cos(_amount * (Mathf.PI / 180)), _v.y, _v.z * Mathf.Cos(_amount * (Mathf.PI / 180)) + _v.x * -Mathf.Sin(_amount * (Mathf.PI / 180)));
    }

    // public List<D> GetPositions() {
    //     Vector4[] toReturn = new Vector3[celestialBodies.Length];
    //     for (int i = 0; i < toReturn.Length; i++) {
    //         toReturn[i] = celestialBodies[i].position.Sub(player.position);
    //     }
    //     return toReturn.ToList();
    // }
    
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
}
