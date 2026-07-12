using UnityEngine;

[System.Serializable]
public class mtrl_containedresource
{
    public string resource_name;

    public float resource_amount;

    public mtrl_containedresource() {}

    public mtrl_containedresource(string resource_name, float resource_amount)
    {
        this.resource_name = resource_name;
        this.resource_amount = resource_amount;
    }

    public mtrl_containedresource(mtrl_containedresource data)
    {
        this.resource_name = data.resource_name;
        this.resource_amount = data.resource_amount;
    }

    public mtrl_generic GetData()
    {
        return ItemManager.GetResourceDataFromName(resource_name);
    }

    public static mtrl_containedresource ParseFromString(string data)
    {
        mtrl_containedresource r = new mtrl_containedresource();

        string[] splitData = util_string.SplitByChar(data, '#');

        r.resource_name = splitData[0];
        r.resource_amount = float.Parse(splitData[1]);

        return r;
    }

    public string FormatAsString()
    {
        string data = resource_name + "#" + resource_amount;
        return data;
    }
}