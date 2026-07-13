using UnityEngine;
using System.Collections.Generic;

// any part that contains items will use this

public class crft_inventory : MonoBehaviour
{
    private crft_genericpart gp;

    public inv_inventorydata data;

    void Awake()
    {
        InitializeInventoryData();

        gp = GetComponent<crft_genericpart>();


        gp.onInitialize.AddListener(Initialize);
    }

    private void Initialize()
    {
        gp.onRecievePartData.AddListener(ProcessPartData);
        gp.partDataCollectors.Add(CreateAdditionalPartData);
    }

    private void InitializeInventoryData()
    {
        data.cellsTaken = new bool[data.inventory_width * data.inventory_height];
        data.items = new List<inv_itemstack>();
    }

    public void OpenInventory()
    {
        UIManager.Instance.OpenInventory();
        ui_inventories.Instance.OpenExternalInventory(() => {return data;});
    }

    #region DATA


    public void ProcessPartData()
    {
        string string_data = gp.GetAdditionalPartData("inventory");
        if (string.IsNullOrEmpty(string_data)) {return;} // should really never happen

        // we really only need a few things here,
        // basically just matches up with the variables up top

        // mind you some are constant, like antenna_range

        string[] splitData = util_string.SplitByChar(string_data, ';');

        // TODO: exception handling for literally all of this

        data.ApplyData(inv_inventorydata.ParseFromString(splitData[0]));
    }

    public string CreateAdditionalPartData()
    {
        string string_data = "inventory:";


        // this is one of the harder ones to do
        // so im putting it all in its own function, this function:
        string_data += data.FormatAsString();
        

        return string_data;
    }


    # endregion
}
