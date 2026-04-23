using UnityEngine;

public class e_fluidcarrier : MonoBehaviour
{
    public e_genericentity e;

    void Awake()
    {
        e = GetComponent<e_genericentity>();
        
    }
    public void UpdateEntityData()
    {
        
    }
}
