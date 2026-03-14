using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cbr_lightsource : MonoBehaviour
{
    void Update() {
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < children.Length; i++) {
            for (int j = 0; j < children[i].sharedMaterials.Length; j++) {
                children[i].sharedMaterials[j].SetVector("sunPos", transform.position);
                children[i].sharedMaterials[j].SetVector("viewDir", transform.position.normalized);
            }
        }
    }
}
