using System;
using UnityEngine;

public class crft_charger : MonoBehaviour
{
    private crft_genericpart gp;

    public crft_resourcecontainer a;
    private int a_partIndex;
    public crft_resourcecontainer b;
    private int b_partIndex;


    // 0 is 'off'
    // 1 is 'push' (b->a)
    // 2 is 'balance' (b=a)
    // 3 is 'feed' (a->b)
    public ushort mode;

    void Awake()
    {
        gp = GetComponent<crft_genericpart>();

        gp.onInitialize.AddListener(Initialize);
    }


    // called upon the player interacting with the charger
    public void DisplayChargerMenu()
    {
        UIManager.Instance.OpenChargerMenu(this);
    }

    void Initialize()
    {
        gp = GetComponent<crft_genericpart>();

        gp.onRecievePartData.AddListener(ProcessPartData);
        gp.partDataCollectors.Add(CreateAdditionalPartData);

        gp.eComp.onCraftBuilt.AddListener(SetConnectingPartReferences);
    }
    

    // once the craft is finished building itself, we take our part indices and turn them into references
    // we have to wait until the craft is built, otherwise the parts may not exist (obv)
    public void SetConnectingPartReferences()
    {
        a = gp.eComp.parts[a_partIndex].GetComponent<crft_resourcecontainer>();
        b = gp.eComp.parts[b_partIndex].GetComponent<crft_resourcecontainer>();
    }

    #region DATA

    public void ProcessPartData()
    {
        string data = gp.GetAdditionalPartData("charger");
        if (string.IsNullOrEmpty(data)) {return;} // should really never happen

        // we really only need a few things here,
        // basically just matches up with the variables up top

        // mind you some are constant, like antenna_range

        string[] splitData = util_string.SplitByChar(data, ';');

        // TODO: exception handling for literally all of this

        // we need three things here: mode, and part indices for the two connecting parts
        mode = ushort.Parse(splitData[0]);

        a_partIndex = int.Parse(splitData[1]);
        b_partIndex = int.Parse(splitData[2]);

        // we of course need to turn these numbers into references for quick access,
        // this is done as soon as the craft is finished building itself 
    }

    public string CreateAdditionalPartData()
    {
        string data = "charger:";

        data += mode + ";";
        data += gp.eComp.GetPartIndexOf(a.gp) + ";";
        data += gp.eComp.GetPartIndexOf(b.gp);

        return data;
    }


    #endregion
}
