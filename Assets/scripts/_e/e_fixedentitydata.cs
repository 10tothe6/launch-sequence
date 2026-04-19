using UnityEngine;

// see entities README!!!

[System.Serializable]
public class e_fixedentitydata
{
    public e_genericentity generic;
    // eventually we will have some data here that isn't generic, but for now the generic part handles it

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
        
        if (LocalPlayer.IsControllingEntity()) {
            if (LocalPlayer.localClient.controllingEntity == generic)
            {
                return;
            }
            num_precisevector3 pos = generic.data.GetPosition();

            // set the transform's position basee on the world offset
            generic.data.reference.position = pos.Add(cb_renderingmanager.Instance.worldOffset).ToVector3();

            // get the position of the camera
            num_precisevector3 camPosition = LocalPlayer.localClient.controllingEntity.data.GetPosition().Add(CameraController.Instance.PositionRelativeToControlEntity());

            // if (generic.gameObject.name == "Moon 0.0")
            // {
            //     Debug.Log("a" + generic.data.GetPosition().AsString());
            //     Debug.Log("b" + generic.data.localPosition.AsString());
            //     Debug.Log("c" + camPosition.Sub(pos).Mag().AsDouble());
            // }
            if (camPosition.Sub(pos).Mag().AsDouble() > cb_renderingmanager.Instance.secondaryCullingRadius + 1)
            {
                // do not render at all
            }
            else
            {
                generic.data.reference.localScale = Vector3.one * 1;
                generic.data.reference.position = generic.data.localPosition.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(LocalPlayer.localClient.controllingEntity.data.reference.position)).ToVector3();
            }
        }

        // if (this != RenderingManager.Instance.entityInControl)
        // {
        //     rotation += new Vector3(0, RenderingManager.Instance.planetRotSpeed, 0);
        //     reference.eulerAngles = rotation;
        // }
    }
}
