using UnityEngine;

public class cbr_litbody : MonoBehaviour
{
    // so that we don't need to call the expensive GetComponentsInChildren<MeshRenderer>() every frame
    private MeshRenderer[] children;
    void Start()
    {
        children = GetComponentsInChildren<MeshRenderer>();

        // we dont really need to update the position every frame,
        // but I want to for now to make my life easier

        // as for the isStar field, we for sure do not need to do it every frame so we do it once, here
        for (int i = 0; i < children.Length; i++) {
            for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                if (GetComponent<cb_trackedbody>().data.bodyType == (ushort)cb_bodytype.Stellar)
                {
                    children[i].materials[j].SetInt("isStar",1);
                }
            }
        }
    }
    void Update() {
        for (int i = 0; i < children.Length; i++) {
            for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                children[i].materials[j].SetVector("position", GetComponent<cb_trackedbody>().pose.data.GetPosition().ToVector3());
            }
        }
    }
}
