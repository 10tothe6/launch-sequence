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

    public void SetData(string data)
    {
        string[] dataElements = util_string.SplitByChar(data,',');
        
    }
}
