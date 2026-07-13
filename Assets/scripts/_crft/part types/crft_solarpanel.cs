using UnityEngine;

// I have no idea what I'm going to do when I need to add solar panel arrays
// maybe just make them a collection of parts?

// we only have support for one panel direction and i guess technically even though they are many panels,
// they do all face the same direction


public class crft_solarpanel : MonoBehaviour
{





    
    [Header("CONFIG")]
    // assuming we are looking directly at the sun with nothing blocking the panel
    // and also the star has a brightness value of 1.00
    public float power_per_second; 


    public Transform t_panelDirection;

    private crft_genericpart gp;

    void Awake()
    {
        gp = GetComponent<crft_genericpart>();

        gp.onInitialize.AddListener(Initialize);
    }

    private void Initialize()
    {
        gp.onRecievePartData.AddListener(ProcessPartData);
        gp.partDataCollectors.Add(CreateAdditionalPartData);

        gp.eComp.onPeriodicUpdate.AddListener(AddPowerToSystem);
    }


    // this does NOT just return the angle to the sun
    // it returns a value [0..1] representing how much power,
    // as a percentage,
    // the solar panel should be making
    // BASED on the angle to the sun
    public float GetAngleFactor()
    {
        // this will break if we ever add binary (or more) star systems
        // TODO: better way of figuring out where the sun(s) are/is?
        num_precisevector3 dir = cb_solarsystem.Instance.monoBodies[1].pose.data.GetPosition().Sub(gp.eComp.GetComponent<e_genericentity>().data.GetPosition());

        Vector3 dir_normalized = dir.Norm().ToVector3();


        // since these are both normalized vectors it will return the Cos of the angle between the panels direction and the sun
        // which is what we want
        return Vector3.Dot(t_panelDirection.forward, dir_normalized);
    }


    // note that we cannot use Time.deltaTime here, 
    // because we need the amount of time since the entity last updated
    // (not the amount of time since the last frame)
    private void AddPowerToSystem()
    {
        float power_to_add = power_per_second * gp.eComp.time_since_last_update;

        // we of course multiply by the cosine of the angle to the sun (the dot product)

        power_to_add *= GetAngleFactor();
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
