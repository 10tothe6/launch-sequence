using UnityEngine;

public class crft_solarpanel : MonoBehaviour
{

    private crft_genericpart gp;

    void Awake()
    {
        gp = GetComponent<crft_genericpart>();

        gp.onRecievePartData.AddListener(ProcessPartData);
        gp.eComp.partDataCollectors.Add(CreateAdditionalPartData);
    }

    #region DATA

    public void ProcessPartData()
    {
        string data = gp.GetAdditionalPartData("antenna");
        if (string.IsNullOrEmpty(data)) {return;} // should really never happen

        // we really only need a few things here,
        // basically just matches up with the variables up top

        // mind you some are constant, like antenna_range

        string[] splitData = util_string.SplitByChar(data, ';');

        // TODO: exception handling for literally all of this

        
    }

    public string CreateAdditionalPartData()
    {
        string data = "";

        return data;
    }

    # endregion
}
