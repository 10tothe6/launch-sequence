using UnityEngine;

// hopefully this doesn't exist already
public enum e_entitytype
{
    Fixed,
    Floating,
    Mimic,
}

// data that goes across all entity types
public class e_generic : MonoBehaviour
{
    public string entityName;
    public ushort type;

    public e_floatingentity floating_ref;
    public e_fixedentity fixed_ref;
    public e_mimicentity mimic_ref;


    public num_precisevector3 GetPosition()
    {
        if (type == (ushort)e_entitytype.Fixed)
        {
            return fixed_ref.data.GetPosition();
        } else if (type == (ushort)e_entitytype.Floating)
        {
            return floating_ref.data.GetPosition();
        } else if (type == (ushort)e_entitytype.Mimic)
        {
            return mimic_ref.data.GetPosition();
        }

        // we should never get here
        return new num_precisevector3(0,0,0);
    }

    public int GetEntityIndex()
    {
        int result = -1; // should never actually return this, just a placeholder

        if (type == 0) // fixed
        {
            result = fixed_ref.data.index;
        } else if (type == 1) // floating
        {
            result = floating_ref.data.index;
        } else if (type == 2) // mimic
        {
            result = mimic_ref.data.index;
        }

        return result;
    }

    public int GetPrefabIndex()
    {
        int result = -1;
        if (type == 0) // fixed
        {
            result = fixed_ref.data.GetPrefabIndex();
        } else if (type == 1) // floating
        {
            result = floating_ref.data.GetPrefabIndex();
        } else if (type == 2) // mimic
        {
            result = mimic_ref.data.GetPrefabIndex();
        }

        return result;
    }

    public string GetData()
    {
        string result = "";
        if (type == 0) // fixed
        {
            result += fixed_ref.data.Package();
        } else if (type == 1) // floating
        {
            result += floating_ref.data.Package();
        } else if (type == 2) // mimic
        {
            result += mimic_ref.data.Package();
        }

        return result;
    }

    public void SetData(string data)
    {
        string[] dataElements = util_string.SplitByChar(data,',');

    }
}
