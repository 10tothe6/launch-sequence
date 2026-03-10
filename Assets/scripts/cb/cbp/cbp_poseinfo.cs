using UnityEngine;

[System.Serializable]
public class cbp_poseinfo
{
    // these vars are used so I don't have to do math more than once
    public DoubleVector3 localPosition;
    public DoubleVector3 velocity;
    
    public cb_trackedbody generic;

    // using physics stuff to move the planets is better for short timespans
    public void StepNewtonian(float delta, float parentMass)
    {
        localPosition = localPosition.Add(velocity.Mul(delta));

        // fix this
        velocity = velocity.Add(localPosition.Mul(-1).Norm().Mul(delta).Mul(cb_solarsystem.gravConstant).Mul(parentMass).Div(localPosition.Mag()).Div(localPosition.Mag()));
    }

    public DoubleVector3 GetPosition()
    {
        if (generic.data.pConfig.parent != null)
        {
            // god i hate this line of code
            // TODO: improve structure
            return localPosition.Add(generic.data.pConfig.parent.data.pConfig.pose.GetPosition());
        }
        else
        {
            return localPosition;
        }
    }
}
