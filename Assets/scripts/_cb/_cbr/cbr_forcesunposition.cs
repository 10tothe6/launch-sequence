using UnityEngine;

public class cbr_forcesunposition : MonoBehaviour
{
    public MeshRenderer[] children; // can be set directly in the inspector
    public Transform sun;

    // Update is called once per frame
    void Update()
    {
        UpdateChildren();

        for (int i = 0; i < children.Length; i++) {
                for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                    children[i].materials[j].SetVector("sunPosition", sun.position.normalized * 10000);
                }
            }
    }

    public void UpdateChildren()
    {
        children = GetComponentsInChildren<MeshRenderer>();
    }
}
