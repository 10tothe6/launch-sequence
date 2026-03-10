using UnityEngine;

[System.Serializable]
public class cbp_poseinfo
{
    // these vars are used so I don't have to do math more than once
    public DoubleVector3 localPosition;
    public DoubleVector3 velocity;

    public trackedbody_mono data;

    // using physics stuff to move the planets is better for short timespans
    public void StepNewtonian(float delta)
    {
        localPosition = localPosition.Add(velocity.Mul(delta));

        // fix this
        velocity = velocity.Add(localPosition.Mul(-1).Norm().Mul(delta).Mul(Sys.gravConstant).Mul(data.config.parentConfig.data.config.mass).Div(localPosition.Mag()).Div(localPosition.Mag()));
    }

    public DoubleVector3 GetPosition()
    {
        if (data.config.parentIndex != -1)
        {
            return localPosition.Add(data.config.parentConfig.data.pose.GetPosition());
        }
        else
        {
            return localPosition;
        }
    }
}
