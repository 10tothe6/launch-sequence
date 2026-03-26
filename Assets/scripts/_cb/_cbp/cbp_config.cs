using UnityEngine;

// config specifically for orbital parameters

// TODO: transfer some values over to cbp_orbit.cs

[System.Serializable]
public class cbp_config
{
    public bool isGrandparent; // sun don't move 
    public int parentIndex; // -1 if no parent
    public int selfIndex; // because I'm not putting mono refs in a data class, this is how things will be referenced

    public cbp_orbit orbit;

    // i for initial
    public Vector3 iPosition;
    public Vector3 iVelocity;

    public float iRadius;
    public float iAngle;
    public float iRadialVelocity;
    public float iTransverseVelocity;
    public float iPhaseShift;
    public float iM;
    public float iN;

    public cbp_config()
    {
        orbit = new cbp_orbit();
    }

    // using physics stuff to move the planets is better for short timespans
    public void StepNewtonian(float delta)
    {
        Pose().localPosition = Pose().localPosition.Add(Pose().velocity.Mul(delta));

        Pose().velocity = Pose().velocity.Add(Pose().localPosition.Mul(-1).Norm().Mul(delta).Mul(cb_solarsystem.gravConstant).Mul(cb_solarsystem.Instance.monoBodies[parentIndex].data.mass).Div(Pose().localPosition.Mag()).Div(Pose().localPosition.Mag()));
    }

    // a very interesting solution to a verbosity problem
    public e_floatingentitydata Pose()
    {
        return cb_solarsystem.Instance.monoBodies[parentIndex].pose.data;
    }

    public DoubleVector3 GetPosition()
    {
        if (parentIndex != -1)
        {
            // god i hate this line of code
            // TODO: improve structure
            return Pose().localPosition.Add(cb_solarsystem.Instance.monoBodies[parentIndex].data.pConfig.GetPosition());
        }
        else
        {
            return Pose().localPosition;
        }
    }


    // everything below here is the really complicated orbital math that I REALLY never want to do it again
    // let's all pray that it works
    // ********************************

    // the polar function for an elipse, adapted
    public float DistFromFocus(float angle)
    {
        return 1 / (iM + iN * Mathf.Cos(angle + iPhaseShift));
    }

    public float MeanAnomaly(float time)
    {
        return Mathf.Pow((iM * iM - iN * iN), 3f / 2f) / iM * iRadius * iTransverseVelocity * time + 2 * Mathf.Atan(Mathf.Sqrt((iM - iN) / (iM + iN)) * Mathf.Tan((iAngle + iPhaseShift) / 2)) - orbit.orbitalEccentricity * Mathf.Sqrt(iM * iM - iN * iN) * Mathf.Sin(iAngle + iPhaseShift) * DistFromFocus(iAngle);
    }

    public float EccentricAnomaly(float time, int p)
    {
        float meanAnomaly = MeanAnomaly(time);

        int n = p; // precision of integral
        float step = (Mathf.PI - 0) / (float)n;
        float sum = (EccentricIntegrationValue(0, meanAnomaly) + EccentricIntegrationValue(Mathf.PI, meanAnomaly)) * 0.5f;

        for (int i = 1; i < n; i++)
        {
            sum += EccentricIntegrationValue((float)i * (float)step, meanAnomaly);
        }

        return sum * step;
    }

    public float EccentricIntegrationValue(float phi, float meanAnomaly)
    {
        return Mathf.Floor((phi - orbit.orbitalEccentricity * Mathf.Sin(phi) + meanAnomaly) / (Mathf.PI * 2f)) - Mathf.Floor((phi - orbit.orbitalEccentricity * Mathf.Sin(phi) - meanAnomaly) / (Mathf.PI * 2f));
    }

    public float TrueAnomaly(float time, int p)
    {
        float eccentricAnomaly = EccentricAnomaly(time, p);

        return 2 * Mathf.Atan(Mathf.Tan(eccentricAnomaly / 2f) * Mathf.Sqrt((iM + iN) / (iM - iN))) - iPhaseShift;
    }

    public DoubleVector3 GetPositionAtTime(float time, int precision)
    {
        float trueAnomaly = TrueAnomaly(time, precision);
        float radius = DistFromFocus(trueAnomaly);

        Vector3 result = new Vector3(radius * Mathf.Cos(trueAnomaly), 0, radius * Mathf.Sin(trueAnomaly));
        //data.pose.position = new DoubleVector3(result).Add(parent.data.pose.position);
        return new DoubleVector3(result);
    }

    public Vector3[] SampleFullOrbit(float scaleFactor, int pointCount)
    {
        Vector3[] result = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            float currentTime = orbit.orbitalPeriod / ((float)pointCount - 1f) * (float)i;

            result[i] = GetPositionAtTime(currentTime, 1000).ToVector3() * scaleFactor;
        }

        return result;
    }
}
