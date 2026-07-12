using UnityEngine;

[System.Serializable]
public class mtrl_resourcecapacity
{
    public string resource_name;

    public float resource_amount;
    public float max_available_space;

    public mtrl_resourcecapacity() {}

    public mtrl_resourcecapacity(string resource_name)
    {
        this.resource_name = resource_name;
        this.resource_amount = 0;
        this.max_available_space = 0;
    }

    public mtrl_resourcecapacity(string resource_name, float resource_amount)
    {
        this.resource_name = resource_name;
        this.resource_amount = resource_amount;
    }

    public mtrl_resourcecapacity(string resource_name, float resource_amount, float max_available_space)
    {
        this.resource_name = resource_name;
        this.resource_amount = resource_amount;
        this.max_available_space = max_available_space;
    }
}
