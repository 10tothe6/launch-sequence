using System.Collections.Generic;
using UnityEditor.Embree;
using UnityEngine;

public class crft_resourcecontainer : MonoBehaviour
{

    public crft_genericpart gp;

    public List<crft_resourcecompartement> compartements;
    
    // each resource container has a set of connections, 
    // and together they all make up a resource network

    // this is especially useful for batteries
    public List<crft_resourcecontainer> connected_containers;
    private List<int> connected_container_indices;


    void Awake()
    {
        gp = GetComponent<crft_genericpart>();

        gp.onInitialize.AddListener(Initialize);
        
    }

    private void Initialize()
    {
        gp.onRecievePartData.AddListener(ProcessPartData);
        gp.partDataCollectors.Add(CreateAdditionalPartData);
    }


    // triggered upon the player interacting with the container
    public void DisplayContainerContents()
    {
        UIManager.Instance.OpenCanisterMenu(this);
    }


    // wrapper
    public void AddResource(string name, float amt)
    {
        AddResource(new mtrl_containedresource(name, amt));
    }
    private void AddResource(mtrl_containedresource resource)
    {
        for (int i = 0; i < compartements.Count; i++)
        {
            mtrl_containedresource leftovers = compartements[i].AddResource(resource);

            resource.resource_amount = leftovers.resource_amount;

            if (resource.resource_amount <= 0)
            {
                return;
            }
        }
    }

    // wrapper
    public void RemoveResource(string name, float amt)
    {
        RemoveResource(new mtrl_containedresource(name, amt));
    }
    private void RemoveResource(mtrl_containedresource resource)
    {
        for (int i = 0; i < compartements.Count; i++)
        {
            mtrl_containedresource leftovers = compartements[i].RemoveResource(resource);

            resource.resource_amount = leftovers.resource_amount;

            if (resource.resource_amount <= 0)
            {
                return;
            }
        }
    }



    public float GetResourceAmount(string resource_name)
    {
        float sum = 0;

        for (int i = 0; i < compartements.Count; i++)
        {
            sum += compartements[i].GetCurrentResourceAmount(resource_name);
        }

        return sum;
    }

    public float GetResourceCapacity(string resource_name)
    {
        float sum = 0;





        return sum;
    }

    #region DATA 


    public void ProcessPartData()
    {
        string data = gp.GetAdditionalPartData("resource_container");
        if (string.IsNullOrEmpty(data)) {return;} // should really never happen

        // we really only need a few things here,
        // basically just matches up with the variables up top

        // mind you some are constant, like antenna_range

        string[] splitData = util_string.SplitByChar(data, '%');

        // TODO: exception handling for literally all of this

        string[] connection_references = new string[0];
        string[] compartements_data = new string[0];

        if (splitData.Length > 1)
        {
            connection_references = util_string.SplitByChar(splitData[0], ';');
            compartements_data = util_string.SplitByChar(splitData[1], ';');
        } else if (splitData.Length > 0)
        {
            compartements_data = util_string.SplitByChar(splitData[0], ';');
        }

        connected_container_indices = new List<int>();
        for (int i = 0; i < connection_references.Length; i++)
        {
            // these will be converted into proper script references later 
            connected_container_indices.Add(int.Parse(connection_references[i]));
        }

        for (int i = 0; i < compartements_data.Length; i++)
        {
            compartements[i].FillWithResources(crft_resourcecompartement.ParseFromString(compartements_data[i]));
        }
    }

    public string CreateAdditionalPartData()
    {
        string data = "resource_container:";

        // first, the connected containers
        for (int i = 0; i < connected_containers.Count; i++)
        {
            data += gp.eComp.GetPartIndexOf(connected_containers[i].gp);
            if (i < connected_containers.Count - 1)
            {
                data += ";"; // the last entry doesn't need a separator char ofc
            }
        }

        // splitting the references from the compartements
        data += "%"; // i'm literally just using random separator chars atp

        for (int i = 0; i < compartements.Count; i++)
        {
            data += compartements[i].FormatAsString();
            if (i < compartements.Count - 1)
            {
                data += ";";
            }
        }

        return data;
    }


    #endregion
}
