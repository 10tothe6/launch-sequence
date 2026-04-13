using UnityEngine;

[System.Serializable]
public class e_genericentitydata
{
    public Transform reference;

    public ushort entityType; // fixed, floating, mimic
    public ushort entityPrefabIndex; // what prefab is this entity using?

    // the in-game index of the entity
    // a unique identifier, basically
    public int index;
    // the name of the entity, helps when trying to refer to it
    // (gameobject name will always be "e_" plus the entity name)
    public string entityName;

    public num_precisevector3 localPosition;
    public e_genericentity parent;
    // precision is only needed here for position
    // it's faster for calculations to keep rotation not precise
    // oh also, I can't be bothered to make a precise quaternion class
    public Quaternion rotation;

    // velocity COULD just be a normal vector3, but may as well do things right
    public num_precisevector3 velocity;

    // MORE ADVANCED DATA
    // ****************************
    public e_fixedentitydata fixedData;
    public e_floatingentitydata floatingData;
    public e_mimicentitydata mimicData;
    // ****************************

    public string GetRawPackagedData()
    {
        return "";
    }

    public net_packagedentitydata GetPackagedData()
    {
        net_packagedentitydata result = new net_packagedentitydata();

        result.data = GetRawPackagedData();
        result.entityPrefabIndex = entityPrefabIndex;

        return result;
    }

    public void SetPackagedData(string data)
    {
        
    }

    public num_precisevector3 GetPosition()
    {
        if (parent == null)
        {
            return localPosition;
        } else
        {
            return parent.data.GetPosition().Add(localPosition);
        }
    }
}
