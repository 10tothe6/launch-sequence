using System.Data.Common;
using UnityEngine;

public class cb_trackedbody : MonoBehaviour
{
    public cb_trackedbodydata data;
    public float epsilon;

    public void Initialize(string name, int parentIndex, ushort bodyType,float baseRadius)
    {
        gameObject.name = name;

        data = new cb_trackedbodydata();

        data.pConfig.isGrandparent = data.bodyType == (ushort)cb_bodytype.Null;
        data.pConfig.parentIndex = parentIndex;
        data.pConfig.selfIndex = cb_solarsystem.Instance.monoBodies.Count - 1;
        data.name = name;

        GenerateOrbit(baseRadius);

        FillDataBasedOnBodyType(bodyType);
    }

    // coming up with all the orbital params from the one base radius
    // this is actually all the data we need
    public void GenerateOrbit(float baseRadius)
    {
        // we can figure out orbital velocity using the newtonian formula
        
        // note - because we generate all planets before any moons and the star before all planets,
        // we can know that the parent body's data will always be available
        if (data.pConfig.parentIndex == -1)
        {
            return;
        }
        float orbitalVelocity = Mathf.Sqrt(cb_solarsystem.gravConstant * cb_solarsystem.Instance.monoBodies[data.pConfig.parentIndex].data.mass / baseRadius);

        // (isGrantparent and parentIndex and selfIndex have already been set)

        // **** enter really complex orbital math that I haven't double-checked since the last time ****
        // aka.
        // **** the wall of math ****

        // ORBITAL PARAMS:
        // we CANNOT use unity positions because those won't be reliable
        // remember engine space vs. game space?

        // also assuming everything happens in the x, z plane (for now)

        // all planets will start on the +X axis, and have velocity in the +Z axis
        data.pConfig.iVelocity = Vector3.forward * orbitalVelocity;
        data.pConfig.iPosition = Vector3.right * baseRadius + Vector3.forward * (baseRadius > 0 ? epsilon : 0);

        // double-check ALL of this
        // TODO: remove all the 'data.pConfig.' prefixes if possible
        data.pConfig.iRadius = data.pConfig.iPosition.magnitude;
        data.pConfig.iAngle = Mathf.Atan2(data.pConfig.iPosition.z, data.pConfig.iPosition.x);

        data.pConfig.iRadialVelocity = data.pConfig.iVelocity.z * Mathf.Sin(data.pConfig.iAngle) + data.pConfig.iVelocity.x * Mathf.Cos(data.pConfig.iAngle);
        data.pConfig.iTransverseVelocity = data.pConfig.iVelocity.z * Mathf.Cos(data.pConfig.iAngle) - data.pConfig.iVelocity.x * Mathf.Sin(data.pConfig.iAngle);

        data.pConfig.iM = cb_solarsystem.gravConstant * cb_solarsystem.Instance.monoBodies[data.pConfig.parentIndex].data.mass / data.pConfig.iRadius / data.pConfig.iRadius / data.pConfig.iTransverseVelocity / data.pConfig.iTransverseVelocity;

        data.pConfig.iN = Mathf.Sqrt(((1 / data.pConfig.iRadius) - data.pConfig.iM) * ((1 / data.pConfig.iRadius) - data.pConfig.iM) + (data.pConfig.iRadialVelocity / data.pConfig.iRadius / data.pConfig.iTransverseVelocity) * (data.pConfig.iRadialVelocity / data.pConfig.iRadius / data.pConfig.iTransverseVelocity));

        data.pConfig.iPhaseShift = Mathf.Sign(data.pConfig.iRadialVelocity * data.pConfig.iTransverseVelocity) * Mathf.Acos(((1 / data.pConfig.iRadius) - data.pConfig.iM) / data.pConfig.iN) - data.pConfig.iAngle;

        data.pConfig.orbit.orbitalEccentricity = data.pConfig.iN / data.pConfig.iM;
        data.pConfig.orbit.orbitalPeriod = (data.pConfig.iM * 2 * Mathf.PI) / (Mathf.Abs(data.pConfig.iTransverseVelocity) * data.pConfig.iRadius * Mathf.Pow(data.pConfig.iM * data.pConfig.iM - data.pConfig.iN * data.pConfig.iN, 1.5f));

        data.pConfig.pose.localPosition = new DoubleVector3(data.pConfig.iPosition);
        data.pConfig.pose.velocity = new DoubleVector3(data.pConfig.iVelocity);
    }

    public void FillDataBasedOnBodyType(ushort type)
    {
        data.bodyType = type;
        data.hasSurface = !(type == (ushort)cb_bodytype.Jovian);
    }
}
