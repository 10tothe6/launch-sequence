using UnityEngine;
using UnityEngine.SocialPlatforms;

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

    public num_precisevector3 localPosition {get; private set;}
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

    public void SetPosition(num_precisevector3 pos)
    {
        localPosition = pos;
        // make sure that the change is communicated to all clients
        if (ServerNetworkManager.Instance.isServerActive)
        {
            // I don't care about planets
            bool doICare = true;
            if (floatingData != null)
            {
                if (floatingData.isCelestial)
                {
                    doICare = false;
                }
            }

            if (doICare)
            {
                ServerNetworkManager.Instance.SendEntityPositionUpdates(new int[] {index});
            }
        }
    }

    public string GetRawPackagedData()
    {
        string result = "";

        result += index;
        result += ":";
        result += entityName;
        result += ":";
        result += localPosition.AsRawString();

        return result;
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
        string[] split = util_string.SplitByChar(data, ':');

        index = int.Parse(split[0]);
        entityName = split[1];

        localPosition = num_precisevector3.FromString(split[2]);
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
