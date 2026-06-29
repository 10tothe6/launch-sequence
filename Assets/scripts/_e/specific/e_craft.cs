using UnityEngine;

public class e_craft : MonoBehaviour
{
    public Transform t_partContainer;
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
        GameObject g_newPart = Instantiate(PartManager.Instance.GetPartPrefabFromName(partData.partName));
        g_newPart.transform.SetParent(t_partContainer);

        g_newPart.transform.localPosition = partData.position;
    }
}
