using UnityEngine;

public class cbr_litbody : MonoBehaviour
{
    // so that we don't need to call the expensive GetComponentsInChildren<MeshRenderer>() every frame
    public MeshRenderer[] children; // can be set directly in the inspector
    public bool useSpecificObjects;
    public bool useIndex;
    public int bodyIndex;
    void Start()
    {
        UpdateChildren();

        // we dont really need to update the position every frame,
        // but I want to for now to make my life easier

        // as for the isStar field, we for sure do not need to do it every frame so we do it once, here
        if (useIndex)
        {
            if (cb_solarsystem.Instance.monoBodies[bodyIndex].data.bodyType == (ushort)cb_bodytype.Stellar)
            {
                for (int i = 0; i < children.Length; i++) {
                    for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                        children[i].materials[j].SetInt("isStar",1);
                    }
                }
            }
        } else
        {
            if (GetComponent<cb_trackedbody>().data.bodyType == (ushort)cb_bodytype.Stellar)
            {
                for (int i = 0; i < children.Length; i++) {
                    for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                        children[i].materials[j].SetInt("isStar",1);
                    }
                }
            }
        }
    }
    void Update() {
        if (useIndex)
        {
            for (int i = 0; i < children.Length; i++) {
                for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                    children[i].materials[j].SetVector("position", cb_solarsystem.Instance.monoBodies[bodyIndex].pose.data.GetPosition().ToVector3());
                    children[i].materials[j].SetVector("worldPosition", transform.position);
                }
            }
        } else
        {
            for (int i = 0; i < children.Length; i++) {
                for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                    children[i].materials[j].SetVector("position", GetComponent<cb_trackedbody>().pose.data.GetPosition().ToVector3());
                    children[i].materials[j].SetVector("worldPosition", Vector3.zero);
                }
            }
        }
    }

    public void UpdateChildren()
    {
        if (!useSpecificObjects)
        {
            children = GetComponentsInChildren<MeshRenderer>();
        }
    }
}
