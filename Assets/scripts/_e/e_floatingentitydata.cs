using UnityEngine;

// see entities README!!!

// Data class for any object being rendered with floating origin + scale
// the player, planets, ships
[System.Serializable]
public class e_floatingentitydata {
    public e_genericentitydata genericData;

    public bool isCelestial; // is it big enough to never un-render?
    public float defaultScale; // the scale the object should be at
    public float scaleFactor;

    public e_floatingentitydata() {scaleFactor = 1;}

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
            num_precisevector3 pos = genericData.GetPosition();

            // set the transform's position basee on the world offset
            genericData.reference.position = pos.Add(cb_renderingmanager.Instance.worldOffset).ToVector3();

            // get the position of the camera
            num_precisevector3 camPosition = cb_renderingmanager.Instance.entityInControl.data.GetPosition().Add(CameraController.Instance.PositionRelativeToControlEntity());

            if (camPosition.Sub(pos).Mag().AsDouble() > cb_renderingmanager.Instance.secondaryCullingRadius + 1)
            {
                if (camPosition.Sub(pos).Mag().AsDouble() < cb_renderingmanager.Instance.inflationRadius)
                {
                    // inflate
                    genericData.reference.localScale = Vector3.one / scaleFactor * defaultScale;
                    genericData.reference.position = genericData.localPosition.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(cb_renderingmanager.Instance.entityInControl.data.reference.position)).ToVector3();
                }
                else
                { // far from planet

                
                genericData.reference.localScale = Vector3.one / scaleFactor * defaultScale * (cb_renderingmanager.Instance.secondaryCullingRadius / (float)camPosition.Sub(genericData.localPosition).Mag().AsDouble());
                genericData.reference.position = pos.Sub(camPosition).Norm().Mul(cb_renderingmanager.Instance.secondaryCullingRadius).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(cb_renderingmanager.Instance.entityInControl.data.reference.position)).ToVector3();


                }
            }
            else
            {
                genericData.reference.localScale = Vector3.one / scaleFactor * defaultScale;
                genericData.reference.position = pos.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(cb_renderingmanager.Instance.entityInControl.data.reference.position)).ToVector3();
            }
        }

        // if (this != RenderingManager.Instance.entityInControl)
        // {
        //     rotation += new Vector3(0, RenderingManager.Instance.planetRotSpeed, 0);
        //     reference.eulerAngles = rotation;
        // }
    }
}