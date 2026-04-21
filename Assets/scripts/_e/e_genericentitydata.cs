using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class e_genericentitydata
{
    public e_genericentity monoComp;

    public Transform reference;

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
    public Quaternion rotation;

    // velocity COULD just be a normal vector3, but may as well do things right
    public num_precisevector3 velocity;

    // DATA SYSTEM (the backbone of the entire project, more or less)
    // very similar, in concept at least, to minecraft's NBT data system
    public List<string> dataKeys {get; private set;}
    public List<string> dataValues {get; private set;}
    public List<string> updatedDataKeys {get; private set;} // updated since the last packet went out

    public bool HasUpdatedValues()
    {
        return updatedDataKeys.Count > 0;
    }
    public void ClearUpdatedData()
    {
        updatedDataKeys.Clear();
    }

    // data coming down from the server, parsed using the format used in the function directly below this one
    public void UpdateData(string data)
    {
        string[] splitByEntry = util_string.SplitByChar(data,'|');

        // first, handle position, rotation and all the other normal stuff
        localPosition = num_precisevector3.FromString(splitByEntry[0]);
        velocity = num_precisevector3.FromString(splitByEntry[1]);
        rotation = util_string.ParseQuaternion(splitByEntry[2]);

        // start at 3 cuz that's where the variable data begins
        for (int i = 3; i < splitByEntry.Length; i++)
        {
            string[] split = util_string.SplitByChar(splitByEntry[i],':');

            SetDataEntry(split[0], split[1]); // thankfully the value can just stay as a string
        }
    }

    public string GetUpdatedData()
    {
        // '|' splits entries, ':' splits key and value and ',' is for multiple values (like a vector)
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

        // now for the data that is variable
        for (int i = 0; i < updatedDataKeys.Count; i++)
        {
            result += updatedDataKeys[i] + ":";
            result += GetDataEntry(updatedDataKeys[i]) + "|";
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

    public void RefreshRenderedPosition()
    {
        if (LocalPlayer.IsControllingEntity()) {
            
            if (entityType == (ushort)e_entitytype.Floating)
                {
                    if (LocalPlayer.localClient.controllingEntity == monoComp)
                {
                    return;
                }
                num_precisevector3 pos = GetPosition();

                // set the transform's position basee on the world offset
                reference.position = pos.Add(cb_renderingmanager.Instance.worldOffset).ToVector3();

                // get the position of the camera
                num_precisevector3 camPosition = LocalPlayer.localClient.controllingEntity.data.GetPosition().Add(CameraController.Instance.PositionRelativeToControlEntity());

                if (camPosition.Sub(pos).Mag().AsDouble() > cb_renderingmanager.Instance.secondaryCullingRadius + 1)
                {
                    // do not render at all
                }
                else
                {
                    reference.localScale = Vector3.one * 1;
                    reference.position = localPosition.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(LocalPlayer.localClient.controllingEntity.data.reference.position)).ToVector3();
                }
            }

            else if (entityType == (ushort)e_entitytype.Floating)
            {
                float scaleFactor = float.Parse(GetDataEntry("scaleFactor"));
                float defaultScale = float.Parse(GetDataEntry("defaultScale"));


                num_precisevector3 pos = GetPosition();

                // get the position of the camera
                num_precisevector3 camPosition = LocalPlayer.localClient.controllingEntity.data.GetPosition().Add(CameraController.Instance.PositionRelativeToControlEntity());

                // TODO: fix the below hot pile of garbage
                // ******************************************************************************
                if (camPosition.Sub(pos).Mag().AsDouble() > cb_renderingmanager.Instance.secondaryCullingRadius + 1)
                {
                    if (camPosition.Sub(pos).Mag().AsDouble() < cb_renderingmanager.Instance.inflationRadius)
                    {
                        // inflate
                        reference.localScale = Vector3.one / scaleFactor * defaultScale;
                        reference.position = pos.Add(cb_renderingmanager.Instance.worldOffset).ToVector3();
                    }
                    else
                    { // far from planet

                    
                    reference.localScale = Vector3.one / scaleFactor * defaultScale * (cb_renderingmanager.Instance.secondaryCullingRadius / (float)camPosition.Sub(GetPosition()).Mag().AsDouble());
                    reference.position = pos.Sub(camPosition).Norm().Mul(cb_renderingmanager.Instance.secondaryCullingRadius).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(LocalPlayer.localClient.controllingEntity.data.reference.position)).ToVector3();


                    }
                }
                else
                {
                    reference.localScale = Vector3.one / scaleFactor * defaultScale;
                    reference.position = pos.Sub(camPosition).Add(CameraController.Instance.PositionRelativeToControlEntity().Add(LocalPlayer.localClient.controllingEntity.data.reference.position)).ToVector3();
                }
                // ******************************************************************************
            }


            else if (entityType == (ushort)e_entitytype.Mimic)
            {
                
            }

        }
    }

    public void SetPosition(num_precisevector3 pos)
    {
        localPosition = pos;
        
        
        // we originally had an update packet sent from here
        // of course that was dumb (and temp) so that's gone
    }

    public string GetRawPackagedData()
    {
        string result = "";

        result += index;
        result += ":";
        result += localPosition.AsRawString();
        result += ":";
        result += entityPrefabIndex;

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

        //Debug.Log(data);

        index = int.Parse(split[0]);

        localPosition = num_precisevector3.FromString(split[1]);

        entityPrefabIndex = ushort.Parse(split[2]);
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
