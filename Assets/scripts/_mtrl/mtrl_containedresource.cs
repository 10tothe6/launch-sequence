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
}