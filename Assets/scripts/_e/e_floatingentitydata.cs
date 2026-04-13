using UnityEngine;

// see entities README!!!

// Data class for any object being rendered with floating origin + scale
// the player, planets, ships
[System.Serializable]
public class e_floatingentitydata {
    // very important for keeping track of entities in the multiplayer system
    public int index;

    public string entityName;

    public bool isCelestial; // is it big enough to never un-render?
    public Transform reference;

    public e_floatingentity parent; // can very much be null

    public num_precisevector3 localPosition; // THIS IS LOCAL
    public num_precisevector3 rotation;
    public num_precisevector3 velocity;
    public float defaultScale; // the scale the object should be at

    public float scaleFactor;

    public e_floatingentitydata() {scaleFactor = 1;}
    public e_floatingentitydata(Transform _ref) {
        reference = _ref;
        localPosition = new num_precisevector3(_ref.position);
        rotation = new num_precisevector3(_ref.eulerAngles);
        defaultScale = _ref.localScale.x;
        scaleFactor = 1;
        //Refresh();
    }

    public e_floatingentitydata(Transform _ref, num_precisevector3 _pos, num_precisevector3 _rot) {
        reference = _ref;
        localPosition = _pos;
        rotation = _rot;
        defaultScale = _ref.localScale.x;
        scaleFactor = 1;
        //Refresh();
    }

    public e_floatingentitydata(Transform _ref, num_precisevector3 _pos, float _scl) {
        reference = _ref;
        localPosition = _pos;
        rotation = new num_precisevector3( _ref.eulerAngles);
        defaultScale = _scl;
        scaleFactor = 1;
        //Refresh();
    }

    public e_floatingentitydata(Transform _ref, num_precisevector3 _pos, float _scl, bool isCelestial) {
        reference = _ref;
        localPosition = _pos;
        rotation = new num_precisevector3( _ref.eulerAngles);
        defaultScale = _scl;
        this.isCelestial = isCelestial;
        scaleFactor = 1;

        //Refresh();
    }

    public net_packagedentitydata Package()
    {
        // for now just adding the position and the index
        string data = "";

        data += localPosition.AsRawString();

        data += ",";
        data += index;

        net_packagedentitydata result = new net_packagedentitydata(data, GetPrefabIndex());

        return result;
    }

    public int GetPrefabIndex()
    {
        for (int i = 0; i < EntityManager.Instance.p_entities.Length; i++)
        {
            if (EntityManager.Instance.p_entities[i].name == "e_" + entityName)
            {
                return i;
            }
        }

        return -1; // should really never get here?
    }

    public num_precisevector3 GetPosition()
    {
        if (parent == null)
        {
            return localPosition;
        } else
        {
            return parent.data.GetPosition().Add(localPosition);
        }
    }

    // Update the position AND SCALE (unity space) based on position (game space)
    public void Refresh()
    {
        // if (reference.gameObject.GetComponent<Rigidbody>() != null)
        // {
        //     if (RenderingManager.Instance.EntityInsideRenderRadius(this))
        //     {
        //         // for now just set the unity position to game position
        //         position = reference.position - RenderingManager.Instance.GetOriginInGameSpace();
        //         rotation = reference.eulerAngles;
        //         velocity = reference.gameObject.GetComponent<Rigidbody>().velocity;
        //     }
        //     else
        //     { // outside render radius (not doing this yet)
        //         reference.position = position + RenderingManager.Instance.worldOffset;
        //         reference.eulerAngles = rotation;
        //     }
        // }
        // else
        // {

        // }
        
        if (cb_renderingmanager.Instance.entityInControl != null) {
            num_precisevector3 pos = GetPosition();

            // set the transform's position basee on the world offset
            reference.position = pos.Add(cb_renderingmanager.Instance.worldOffset).ToVector3();

            // get the position of the camera
            num_precisevector3 camPosition = cb_renderingmanager.Instance.entityInControl.data.GetPosition().Add(CameraController.Instance.PositionRelativeToControlEntity());

            if (camPosition.Sub(pos).Mag().AsDouble() > cb_renderingmanager.Instance.secondaryCullingRadius + 1)
            {
                if (camPosition.Sub(pos).Mag().AsDouble() < cb_renderingmanager.Instance.inflationRadius)
                {
                    // inflate
                    reference.localScale = Vector3.one / scaleFactor * defaultScale;
                    reference.position = localPosition.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(cb_renderingmanager.Instance.entityInControl.data.reference.position)).ToVector3();
                }
                else
                { // far from planet

                
                reference.localScale = Vector3.one / scaleFactor * defaultScale * (cb_renderingmanager.Instance.secondaryCullingRadius / (float)camPosition.Sub(localPosition).Mag().AsDouble());
                reference.position = pos.Sub(camPosition).Norm().Mul(cb_renderingmanager.Instance.secondaryCullingRadius).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(cb_renderingmanager.Instance.entityInControl.data.reference.position)).ToVector3();


                }
            }
            else
            {
                reference.localScale = Vector3.one / scaleFactor * defaultScale;
                reference.position = pos.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(cb_renderingmanager.Instance.entityInControl.data.reference.position)).ToVector3();
            }
        }

        // if (this != RenderingManager.Instance.entityInControl)
        // {
        //     rotation += new Vector3(0, RenderingManager.Instance.planetRotSpeed, 0);
        //     reference.eulerAngles = rotation;
        // }
    }
}