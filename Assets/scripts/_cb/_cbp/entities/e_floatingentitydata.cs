using UnityEngine;

// see entities README!!!

// Data class for any object being rendered with floating origin + scale
// the player, planets, ships
[System.Serializable]
public class e_floatingentitydata {
    public bool isCelestial; // is it big enough to never un-render?
    public Transform reference;

    public e_floatingentity parent; // can very much be null

    public DoubleVector3 localPosition; // THIS IS LOCAL
    public DoubleVector3 rotation;
    public DoubleVector3 velocity;
    public float defaultScale; // the scale the object should be at

    public e_floatingentitydata() {}
    public e_floatingentitydata(Transform _ref) {
        reference = _ref;
        localPosition = new DoubleVector3(_ref.position);
        rotation = new DoubleVector3(_ref.eulerAngles);
        defaultScale = _ref.localScale.x;
        //Refresh();
    }

    public e_floatingentitydata(Transform _ref, DoubleVector3 _pos, DoubleVector3 _rot) {
        reference = _ref;
        localPosition = _pos;
        rotation = _rot;
        defaultScale = _ref.localScale.x;
        //Refresh();
    }

    public e_floatingentitydata(Transform _ref, DoubleVector3 _pos, float _scl) {
        reference = _ref;
        localPosition = _pos;
        rotation = new DoubleVector3( _ref.eulerAngles);
        defaultScale = _scl;
        //Refresh();
    }

    public e_floatingentitydata(Transform _ref, DoubleVector3 _pos, float _scl, bool isCelestial) {
        reference = _ref;
        localPosition = _pos;
        rotation = new DoubleVector3( _ref.eulerAngles);
        defaultScale = _scl;
        this.isCelestial = isCelestial;

        //Refresh();
    }

    public DoubleVector3 GetPosition()
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
            DoubleVector3 pos = GetPosition();

            // set the transform's position basee on the world offset
            reference.position = pos.Add(cb_renderingmanager.Instance.worldOffset).ToVector3();

            // get the position of the camera
            DoubleVector3 camPosition = cb_renderingmanager.Instance.entityInControl.data.GetPosition().Add(CameraController.Instance.PositionRelativeToControlEntity());

            if (camPosition.Sub(pos).Mag() > cb_renderingmanager.Instance.secondaryCullingRadius + 1)
            {
                // I temporarily(?) removed the inflation thing
                
                // if (camPosition.Sub(pos).Mag() < cb_renderingmanager.Instance.)
                // {
                //     // inflate
                //     reference.localScale = Vector3.one * defaultScale;
                //     reference.position = localPosition.Sub(camPosition).Add(cb_renderingmanager.Instance.entityInControl.data.reference.position + CameraController.Instance.PositionRelativeToControlEntity()).ToVector3();
                // }
                // else
                // { // far from planet

                
                reference.localScale = Vector3.one * defaultScale * (cb_renderingmanager.Instance.secondaryCullingRadius / (float)camPosition.Sub(localPosition).Mag());
                reference.position = localPosition.Sub(camPosition).Norm().Mul(cb_renderingmanager.Instance.secondaryCullingRadius).Add(cb_renderingmanager.Instance.entityInControl.data.reference.position + CameraController.Instance.PositionRelativeToControlEntity()).ToVector3();


                //}
            }
            else
            {
                reference.localScale = Vector3.one * defaultScale;
                reference.position = localPosition.Sub(camPosition).Add(cb_renderingmanager.Instance.entityInControl.data.reference.position + CameraController.Instance.PositionRelativeToControlEntity()).ToVector3();
            }
        }

        // if (this != RenderingManager.Instance.entityInControl)
        // {
        //     rotation += new Vector3(0, RenderingManager.Instance.planetRotSpeed, 0);
        //     reference.eulerAngles = rotation;
        // }
    }
}