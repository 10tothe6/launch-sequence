using UnityEngine;

[System.Serializable]
public class mtrl_containedresource
{
    public string resource_name;

    public float current_capacity;
    public float max_capacity;

    public mtrl_containedresource() {}

    public mtrl_containedresource(string resource_name, float max_capacity)
    {
        this.resource_name = resource_name;
        this.max_capacity = max_capacity;
    }

    public mtrl_containedresource(string resource_name, float current_capacity, float max_capacity)
    {
        this.resource_name = resource_name;
        this.max_capacity = max_capacity;
        this.current_capacity = current_capacity;
    }
}
