using UnityEngine;

public class cb_trackedbody : MonoBehaviour
{
    public cb_trackedbodydata data;

    public void Initialize(string name, int parentIndex, ushort bodyType)
    {
        data = new cb_trackedbodydata();

        data.pConfig.parentIndex = parentIndex;
        data.name = name;

        FillDataBasedOnBodyType(bodyType);
    }

    public void FillDataBasedOnBodyType(ushort type)
    {
        data.bodyType = type;
    }
}
