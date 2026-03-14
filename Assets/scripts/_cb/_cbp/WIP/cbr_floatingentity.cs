using UnityEngine;

// Data class for any object being rendered with floating origin + scale
// the player, planets, ships
[System.Serializable]
public class cbp_floatingentity {
    public Transform reference;
    public DoubleVector3 position;
    public DoubleVector3 rotation;
    public DoubleVector3 velocity;
    public float defaultScale; // the scale the object should be at

    public cbp_floatingentity(Transform _ref) {
        reference = _ref;
        position = new DoubleVector3(_ref.position);
        rotation = new DoubleVector3(_ref.eulerAngles);
        defaultScale = _ref.localScale.x;
        Refresh();
    }

    public cbp_floatingentity(Transform _ref, DoubleVector3 _pos, DoubleVector3 _rot) {
        reference = _ref;
        position = _pos;
        rotation = _rot;
        defaultScale = _ref.localScale.x;
        Refresh();
    }

    public cbp_floatingentity(Transform _ref, DoubleVector3 _pos, float _scl) {
        reference = _ref;
        position = _pos;
        rotation = new DoubleVector3( _ref.eulerAngles);
        defaultScale = _scl;
        Refresh();
    }

    // Update the position (unity space) based on position (game space)
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
        
        if (cbp_renderingmanager.Instance.entityInControl.reference != null) {
            reference.position = position.Add(cbp_renderingmanager.Instance.worldOffset).ToVector3();
            DoubleVector3 camPosition = cbp_renderingmanager.Instance.entityInControl.position.Add(cbp_renderingmanager.Instance.GetCameraOffset());

            if ((camPosition.Sub(position)).Mag() > cbp_renderingmanager.Instance.renderRadius + 1)
            {
                if ((camPosition.Sub(position)).Mag() < 1100f)
                {
                    // inflate
                    reference.localScale = Vector3.one * defaultScale;
                    reference.position = (position.Sub(camPosition)).Add(cbp_renderingmanager.Instance.entityInControl.reference.position + cbp_renderingmanager.Instance.GetCameraOffset()).ToVector3();
                }
                else
                { // far from planet
                    reference.localScale = Vector3.one * defaultScale * (cbp_renderingmanager.Instance.renderRadius / (float)(camPosition.Sub(position)).Mag());
                    reference.position = (position.Sub(camPosition)).Norm().Mul(cbp_renderingmanager.Instance.renderRadius).Add(cbp_renderingmanager.Instance.entityInControl.reference.position + cbp_renderingmanager.Instance.GetCameraOffset()).ToVector3();
                }
            }
            else
            {
                reference.localScale = Vector3.one * defaultScale;
                reference.position = (position.Sub(camPosition)).Add(cbp_renderingmanager.Instance.entityInControl.reference.position + cbp_renderingmanager.Instance.GetCameraOffset()).ToVector3();
            }
        }

        // if (this != RenderingManager.Instance.entityInControl)
        // {
        //     rotation += new Vector3(0, RenderingManager.Instance.planetRotSpeed, 0);
        //     reference.eulerAngles = rotation;
        // }
    }
}