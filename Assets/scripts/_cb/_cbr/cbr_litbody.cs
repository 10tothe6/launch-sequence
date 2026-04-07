using UnityEngine;

public class cbr_litbody : MonoBehaviour
{
    void Update() {
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < children.Length; i++) {
            for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                // children[i].materials[j].SetVector("sunPosition", transform.position);
                children[i].materials[j].SetVector("position", GetComponent<cb_trackedbody>().pose.data.GetPosition().ToVector3());
                if (GetComponent<cb_trackedbody>().data.bodyType == (ushort)cb_bodytype.Stellar)
                {
                    children[i].materials[j].SetInt("isStar",1);
                }
            }
        }
    }
}
