using UnityEngine;

public class cbr_forcesunposition : MonoBehaviour
{
    public bool fullbright;
    public MeshRenderer[] children; // can be set directly in the inspector
    public Transform sun;

    // Update is called once per frame
    void Update()
    {
        UpdateChildren();

        if (fullbright)
        {
            for (int i = 0; i < children.Length; i++) {
                for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                    children[i].materials[j].SetInt("isStar", 1);
                }
            }
        } else
        {
            for (int i = 0; i < children.Length; i++) {
                for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                    children[i].materials[j].SetVector("sunPosition", sun.position.normalized * 10000);
                }
            }
        }
    }

    public void UpdateChildren()
    {
        children = GetComponentsInChildren<MeshRenderer>();
    }
}
