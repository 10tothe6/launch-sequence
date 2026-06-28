using UnityEngine;

public class e_package : MonoBehaviour
{
    private e_genericentity eComp;

    public string containedPartName; // what part is in the package

    void Awake()
    {
        eComp = GetComponent<e_genericentity>();
    }
}
