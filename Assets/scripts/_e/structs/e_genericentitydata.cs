using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class e_genericentitydata
{
    public bool isPhysicsBased;
    public e_genericentity monoComp;

    public ushort entityType; // fixed, floating, mimic
    public ushort state; // see e_possibleentitystates
    public ushort entityPrefabIndex; // what prefab is this entity using?
    // all entities MUST come from a prefab

    // the in-game index of the entity
    // a unique identifier, basically
    public int index;

    public num_precisevector3 localPosition {get; private set;}
    public e_genericentity parent;
    // precision is only needed here for position
    // it's faster for calculations to keep rotation not precise
    // oh also, I can't be bothered to make a precise quaternion class
    public Quaternion rotation {get; private set;}

    // velocity COULD just be a normal vector3, but may as well do things right
    public num_precisevector3 velocity;

    // DATA SYSTEM (the backbone of the entire project, more or less)
    // very similar, in concept at least, to minecraft's NBT data system
    public List<string> dataKeys {get; private set;}
    public List<string> dataValues {get; private set;}
    public List<string> updatedDataKeys {get; private set;} // updated since the last packet went out
    public bool hasTransformBeenUpdated;

    public void SetRotation(Quaternion rot)
    {
        rotation = rot;
        hasTransformBeenUpdated = true;
    }

    public e_genericentitydata()
    {
        dataKeys = new List<string>();
        dataValues = new List<string>();
        updatedDataKeys = new List<string>();
    }

    public bool HasUpdatedValues()
    {
        return updatedDataKeys.Count > 0 || hasTransformBeenUpdated;
    }
    public void ClearUpdatedData()
    {
        updatedDataKeys.Clear();
        hasTransformBeenUpdated = false;
    }

    // data coming down from the server, parsed using the format used in the function directly below this one
    public void UpdateData(string data)
    {
        string[] splitByEntry = util_string.SplitByChar(data,'|');

        num_precisevector3 oldPosition = localPosition;

        // first, handle position, rotation and all the other normal stuff
        localPosition = num_precisevector3.FromString(splitByEntry[0].Substring(splitByEntry[0].IndexOf(':') + 1));
        velocity = num_precisevector3.FromString(splitByEntry[1].Substring(splitByEntry[1].IndexOf(':') + 1));
        rotation = util_string.ParseQuaternion(splitByEntry[2].Substring(splitByEntry[2].IndexOf(':') + 1));

        // making sure that the transform obeys
        monoComp.transform.rotation = rotation;
        
        if (localPosition.Sub(oldPosition).ToVector3().magnitude > 1f || localPosition.Sub(Coord.originPosition).ToVector3().magnitude > 5f)
        {
            Coord.Instance.TeleportEntity(localPosition, monoComp);
        } else
        {
            monoComp.transform.position = Coord.GetUnityPosition(monoComp);
        }

        // start at 3 cuz that's where the variable data begins
        for (int i = 3; i < splitByEntry.Length; i++)
        {
            string[] split = util_string.SplitByChar(splitByEntry[i],':');

            SetDataEntry(split[0], split[1]); // thankfully the value can just stay as a string
        }
    }

    public string BasicData()
    {
        string result = "";

        result += "localPosition:";
        result += localPosition.AsRawString();
        result += "|";
        result += "velocity:";
        result += velocity.AsRawString();
        result += "|";
        result += "rotation:";
        result += util_string.ParseQuaternion(rotation); // maybe change to transform.rotation? having this var seems redundant
        result += "|";

        return result;
    }

    public string GetUpdatedData()
    {
        // '|' splits entries, ':' splits key and value and ',' is for multiple values (like a vector)
        string result = BasicData();

        // now for the data that is variable
        for (int i = 0; i < updatedDataKeys.Count; i++)
        {
            result += updatedDataKeys[i] + ":";
            result += GetDataEntry(updatedDataKeys[i]) + "|";
        }

        return result;
    }

    public string GetRawPackagedData()
    {
        string result = BasicData();

        // now for the data that is variable
        // ALL the data keys
        for (int i = 0; i < dataKeys.Count; i++)
        {
            result += dataKeys[i] + ":";
            result += GetDataEntry(dataKeys[i]) + "|";
        }

        return result;
    }


    public int GetDataEntryIndex(string key)
    {
        for (int i = 0; i < dataKeys.Count; i++)
        {
            if (dataKeys[i] == key)
            {
                return i;
            }
        }

        return -1;
    }

    public string GetDataEntry(string key)
    {
        for (int i = 0; i < dataKeys.Count; i++)
        {
            if (dataKeys[i] == key)
            {
                return dataValues[i];
            }
        }
        return "";
    }

    public void SetDataEntry(string key, string newValue)
    {
        int index = GetDataEntryIndex(key);

        if (index == -1)
        {
            dataKeys.Add(key);
            dataValues.Add(newValue);
        }
        else
        {
            dataValues[index] = newValue;
        }

        if (!updatedDataKeys.Contains(key))
        {
            updatedDataKeys.Add(key);
        }
    }

    public void SetPosition(num_precisevector3 pos)
    {
        localPosition = pos;
        
        hasTransformBeenUpdated = true;
    }

    public net_packagedentitydata GetPackagedData()
    {
        net_packagedentitydata result = new net_packagedentitydata();

        result.data = GetRawPackagedData();
        result.entityPrefabIndex = entityPrefabIndex;

        return result;
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
