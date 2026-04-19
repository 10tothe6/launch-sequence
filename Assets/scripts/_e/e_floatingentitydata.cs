using UnityEngine;

// see entities README!!!

// Data class for any object being rendered with floating origin + scale
// the player, planets, ships
[System.Serializable]
public class e_floatingentitydata {
    public e_genericentity generic;

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
        
        if (LocalPlayer.localClient.controllingEntity != null) {
            num_precisevector3 pos = generic.data.GetPosition();

            // get the position of the camera
            num_precisevector3 camPosition = LocalPlayer.localClient.controllingEntity.data.GetPosition().Add(CameraController.Instance.PositionRelativeToControlEntity());

            // if (generic.gameObject.name == "Moon 0.0")
            // {
            //     Debug.Log("a" + generic.data.GetPosition().AsString());
            //     Debug.Log("b" + generic.data.localPosition.AsString());
            //     Debug.Log("c" + camPosition.Sub(pos).Mag().AsDouble());
            // }

            // TODO: fix the below hot pile of garbage
            // ******************************************************************************
            if (camPosition.Sub(pos).Mag().AsDouble() > cb_renderingmanager.Instance.secondaryCullingRadius + 1)
            {
                if (camPosition.Sub(pos).Mag().AsDouble() < cb_renderingmanager.Instance.inflationRadius)
                {
                    // inflate
                    generic.data.reference.localScale = Vector3.one / scaleFactor * defaultScale;
                    generic.data.reference.position = pos.Add(cb_renderingmanager.Instance.worldOffset).ToVector3();
                }
                else
                { // far from planet

                
                generic.data.reference.localScale = Vector3.one / scaleFactor * defaultScale * (cb_renderingmanager.Instance.secondaryCullingRadius / (float)camPosition.Sub(generic.data.GetPosition()).Mag().AsDouble());
                generic.data.reference.position = pos.Sub(camPosition).Norm().Mul(cb_renderingmanager.Instance.secondaryCullingRadius).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(LocalPlayer.localClient.controllingEntity.data.reference.position)).ToVector3();


                }
            }
            else
            {
                generic.data.reference.localScale = Vector3.one / scaleFactor * defaultScale;
                generic.data.reference.position = pos.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(LocalPlayer.localClient.controllingEntity.data.reference.position)).ToVector3();
            }
            // ******************************************************************************
        }

        // if (this != RenderingManager.Instance.entityInControl)
        // {
        //     rotation += new Vector3(0, RenderingManager.Instance.planetRotSpeed, 0);
        //     reference.eulerAngles = rotation;
        // }
    }
}