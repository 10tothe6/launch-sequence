using UnityEngine;

public class e_craft : MonoBehaviour
{
    private e_genericentity eComp;

    void Awake()
    {
        eComp = GetComponent<e_genericentity>();
    }
}
