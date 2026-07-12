using UnityEngine;

public class crft_antenna : MonoBehaviour
{
    private crft_genericpart gp;

    public float antenna_range;



    // when I say 'ping' I mean the signals that can be picked up by the player's scanner
    [Header("ping settings")]
    [SerializeField]
    private bool is_emitting_ping;
    public float ping_frequency;
    public float ping_range;

    void Awake()
    {
        gp = GetComponent<crft_genericpart>();

        gp.onRecievePartData.AddListener(ProcessPartData);
        gp.eComp.partDataCollectors.Add(CreateAdditionalPartData);
    }

    public bool IsEmittingPing()
    {
        // TODO: also check for electricity
        return is_emitting_ping;
    }

    #region DATA


    // realistaically for these two functions the smart thing would be to make them @Overrides, I suspect
    public void ProcessPartData()
    {
        string data = gp.GetAdditionalPartData("antenna");
        if (string.IsNullOrEmpty(data)) {return;} // should really never happen

        // we really only need a few things here,
        // basically just matches up with the variables up top

        // mind you some are constant, like antenna_range

        string[] splitData = util_string.SplitByChar(data, ';');

        // TODO: exception handling for literally all of this

        is_emitting_ping = bool.Parse(splitData[0]);
        ping_frequency = float.Parse(splitData[1]);
        // ping_range is constant
    }

    public string CreateAdditionalPartData()
    {
        string data = "antenna:";

        // same two variables as function above
        data += is_emitting_ping + ";";
        data += ping_frequency;

        return data;
    }

    #endregion
}
