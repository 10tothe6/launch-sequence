using UnityEngine;

public class e_craft : MonoBehaviour
{
    private e_genericentity eComp;

    public crft_genericpart[] parts;

    void Awake()
    {
        eComp = GetComponent<e_genericentity>();
    }


    // TODO: some way of doing part connections
    public void Initialize(crft_genericpartdata[] partData)
    {
        for (int i = 0; i < partData.Length; i++)
        {
            AddPart(partData[i]);
        }
    }

    public void AddPart(crft_genericpartdata partData)
    {
        
    }
}
