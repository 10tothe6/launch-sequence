using UnityEngine;

public class crft_genericpart : MonoBehaviour
{
    public crft_genericpartdata data;
    public crft_genericpart[] connectedParts;

    public string GetPartName()
    {
        // gets rid of any suffixes, like "(clone)" that unity puts on
        // just making sure, y'know?
        string[] splitName = util_string.SplitByChar(gameObject.name, '(');

        // removing the "part_" prefix
        return splitName[0].Substring(5);
    }
}
