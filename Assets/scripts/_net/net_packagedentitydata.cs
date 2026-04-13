using UnityEngine;

[System.Serializable]
public class net_packagedentitydata
{
    public string data;
    public int entityPrefabIndex; // technically this doesn't NEED to be separate, it just is for convinience

    public net_packagedentitydata() {}

    public net_packagedentitydata(string data, int entityPrefabIndex)
    {
        this.data = data;
        this.entityPrefabIndex =entityPrefabIndex;
    }

    public static string[] MakeDataArray(net_packagedentitydata[] raw)
    {
        string[] result = new string[raw.Length];

        for (int i = 0; i < raw.Length; i++)
        {
            result[i] = raw[i].data;
        }

        return result;
    }
    public static int[] MakeIndexArray(net_packagedentitydata[] raw)
    {
        int[] result = new int[raw.Length];

        for (int i = 0; i < raw.Length; i++)
        {
            result[i] = raw[i].entityPrefabIndex;
        }

        return result;
    }
}
