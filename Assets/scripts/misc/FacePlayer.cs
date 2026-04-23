using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    void Update()
    {
        if (LocalPlayer.IsControllingEntity())
        {
            transform.forward = transform.position - LocalPlayer.localClient.controllingEntity.data.reference.position;
        }
    }
}
