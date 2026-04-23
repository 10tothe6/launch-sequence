using System;
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
        Pose().SetPosition(Pose().localPosition.Add(Pose().velocity.Mul(delta)));

        Pose().velocity = Pose().velocity.Add(Pose().localPosition.Mul(-1).Norm().Mul(delta).Mul(cb_solarsystem.gravConstant).Mul(cb_solarsystem.Instance.monoBodies[parentIndex].data.mass).Div(Pose().localPosition.Mag()).Div(Pose().localPosition.Mag()));
    }

    // a very interesting solution to a verbosity problem
    public e_genericentitydata Pose()
    {
        return cb_solarsystem.Instance.monoBodies[selfIndex].pose.data;
    }

    public num_precisevector3 GetPosition()
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
    public double DistFromFocus(double angle)
    {
        return 1d / (iM + iN * Math.Cos(angle + iPhaseShift));
    }

    public double MeanAnomaly(float time)
    {
        return Mathf.Pow((iM * iM - iN * iN), 3f / 2f) / iM * iRadius * iTransverseVelocity * time + 2 * Mathf.Atan(Mathf.Sqrt((iM - iN) / (iM + iN)) * Mathf.Tan((iAngle + iPhaseShift) / 2)) - orbit.orbitalEccentricity * Mathf.Sqrt(iM * iM - iN * iN) * Mathf.Sin(iAngle + iPhaseShift) * DistFromFocus(iAngle);
    }

    public double EccentricAnomaly(float time, int p)
    {
        double meanAnomaly = MeanAnomaly(time);

        int n = p; // precision of integral
        float step = (Mathf.PI - 0) / (float)n;
        double sum = (EccentricIntegrationValue(0, meanAnomaly) + EccentricIntegrationValue(Mathf.PI, meanAnomaly)) * 0.5f;

        for (int i = 1; i < n; i++)
        {
            sum += EccentricIntegrationValue((float)i * (float)step, meanAnomaly);
        }

        return sum * step;
    }

    public double EccentricIntegrationValue(double phi, double meanAnomaly)
    {
        return Math.Floor((phi - orbit.orbitalEccentricity * Math.Sin(phi) + meanAnomaly) / (Math.PI * 2f)) - Math.Floor((phi - orbit.orbitalEccentricity * Math.Sin(phi) - meanAnomaly) / (Math.PI * 2f));
    }

    public double TrueAnomaly(float time, int p)
    {
        double eccentricAnomaly = EccentricAnomaly(time, p);

        return 2 * Math.Atan(Math.Tan(eccentricAnomaly / 2f) * Math.Sqrt((iM + iN) / (iM - iN))) - iPhaseShift;
    }

    public num_precisevector3 GetPositionAtTime(float time, int precision)
    {
        double trueAnomaly = TrueAnomaly(time, precision);
        double radius = DistFromFocus(trueAnomaly);
        // Debug.Log(radius.AsDouble());
        // Debug.Log("x     " + Math.Cos(trueAnomaly));
        // Debug.Log("x2      " + radius.Mul(Math.Cos(trueAnomaly)).AsDouble());

        num_precisevector3 result = new num_precisevector3(radius * Math.Cos(trueAnomaly), 0, radius * Math.Sin(trueAnomaly));
        //data.pose.position = new DoubleVector3(result).Add(parent.data.pose.position);

        Debug.Log(result.AsRawString());
        return result;
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
