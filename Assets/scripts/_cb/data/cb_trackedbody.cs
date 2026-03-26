using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class cb_trackedbody : MonoBehaviour
{
    public cb_trackedbodydata data;
    public e_floatingentity pose;
    public Transform t_model;
    public float epsilon;

    public void Initialize(string name, int parentIndex, ushort bodyType,float baseRadius)
    {
        data = new cb_trackedbodydata();
        
        gameObject.name = name;
        data.name = name;

        // basic data
        if (bodyType == (ushort)cb_bodytype.Stellar || bodyType == (ushort)cb_bodytype.Null)
        {
            data.mass = 1000000f;
        } else if (bodyType == (ushort)cb_bodytype.Jovian || bodyType == (ushort)cb_bodytype.Terran)
        {
            data.mass = 20f;
        } else
        {
            data.mass = 1f;
        }
        data.pConfig.isGrandparent = data.bodyType == (ushort)cb_bodytype.Null;
        data.pConfig.parentIndex = parentIndex;
        data.pConfig.selfIndex = cb_solarsystem.Instance.monoBodies.Count - 1;
        

        GenerateOrbit(baseRadius);
        FillDataBasedOnBodyType(bodyType);
        // this generates the actual object mesh
        GenerateModel();

        // floating entity parent (only applies to celestial bodies)
        if (parentIndex != -1)
        {
            pose.data.parent = cb_solarsystem.Instance.monoBodies[parentIndex].pose;
        }
    }

    public int GetMoonCount()
    {
        int sum = 0;
        for (int i = 0; i < cb_solarsystem.Instance.monoBodies.Count; i++)
        {
            if (cb_solarsystem.Instance.monoBodies[i].data.pConfig.parentIndex == data.pConfig.selfIndex)
            {
                sum++;
            }
        }
        return sum;
    }

    // the physical mesh that the body will use
    public void GenerateModel()
    {
        t_model.GetChild(0).localScale = Vector3.one * data.tConfig.equitorialRadius * 2;
    }

    // handles: 
    // * min and max cloud radius
    // * default gas types and amounts
    
    // there are some rules for what resources appear in planet atmospheres:
    // terran planets have a CHANCE for an exotic gas, but NEVER MORE THAN ONE
    // jovian plantes ALWAYS HAVE 2 EXOTIC GASSES, chance to have 3 or 4
    //      this is because collecting gas from a jovian planet's atmosphere and bringing it back is incredibly hard
    // moons, of course, have no atmosphere
    public void BuildAtmosphereBasedOnBodyType(ushort type)
    {
        data.tConfig.defaultAtmosphereGasTypes = new List<int>();
        data.tConfig.defaultAtmosphereGasAmounts = new List<float>();

        int exoticGasCount = 0;
        int totalGasCount = 0;

        if (type == (ushort)cb_bodytype.Jovian)
        {
            totalGasCount = Random.Range(
                cb_solarsystem.Instance.minimumJovianGasCount,
                cb_solarsystem.Instance.maximumJovianGasCount
            );

            exoticGasCount = 2;
            
            if (util_math.DiceRoll(cb_solarsystem.Instance.jovianThirdExoticGasChance))
            {
                exoticGasCount = 3;
            }
            if (util_math.DiceRoll(cb_solarsystem.Instance.jovianFourthExoticGasChance))
            {
                exoticGasCount = 4;
            }
            if (util_math.DiceRoll(cb_solarsystem.Instance.jovianFifthExoticGasChance))
            {
                exoticGasCount = 5;
            }
        } 
        else if (type == (ushort)cb_bodytype.Terran)
        {
            totalGasCount = Random.Range(
                cb_solarsystem.Instance.minimumTerranGasCount,
                cb_solarsystem.Instance.maximumTerranGasCount
            );

            exoticGasCount = 0;

            if (util_math.DiceRoll(cb_solarsystem.Instance.terranExoticGasChance))
            {
                exoticGasCount = 1;
            }
        }

        // here is the part where we actually create the exotic gasses
        for (int i = 0; i < exoticGasCount; i++)
        {
            // the thing about exotic gases is that they are not programmed by me
            // so we actually have to make up one right here on the spot

            // exotic gases, to keep things interesting, are only found on a single body
            // they are never re-used over multiple bodies

            data.tConfig.defaultAtmosphereGasTypes.Add(WorldData.Instance.CreateExoticGas());

        }

        // with the exotic gases out of the way, we now do the 'normal' gases
        // like (nitrogen, oxygen, whatever)

        for (int i = 0; i < totalGasCount - exoticGasCount; i++)
        {
            
        }
    }

    public void FillDataBasedOnBodyType(ushort type)
    {
        data.bodyType = type;
        data.hasSurface = !(type == (ushort)cb_bodytype.Jovian);

        // sizes are based on ranges for each planet type
        if (type == (ushort)cb_bodytype.Terran)
        {
            data.tConfig.equitorialRadius = Random.Range(
                cb_solarsystem.Instance.minimumTerranSurfaceRadius,
                cb_solarsystem.Instance.maximumTerranSurfaceRadius
            );

            data.hasAtmosphere = util_math.DiceRoll(cb_solarsystem.Instance.chanceForTerrainAtmosphere);

            if (data.hasAtmosphere)
            {
                data.tConfig.atmosphericRadius = Random.Range(
                    cb_solarsystem.Instance.minimumTerranAtmosphereRadius,
                    cb_solarsystem.Instance.maximumTerranAtmosphereRadius
                );

                BuildAtmosphereBasedOnBodyType(type);
            } else
            {
                data.tConfig.atmosphericRadius = -1;
            }
        } 
        else if (type == (ushort)cb_bodytype.Jovian)
        {
            data.tConfig.equitorialRadius = Random.Range(
                cb_solarsystem.Instance.minimumJovianSurfaceRadius,
                cb_solarsystem.Instance.maximumJovianSurfaceRadius
            );

            data.hasAtmosphere = true; // if you think about it, the planet is ONLY atmosphere

            data.tConfig.atmosphericRadius = Random.Range(
                cb_solarsystem.Instance.minimumJovianAtmosphereRadius,
                cb_solarsystem.Instance.maximumJovianAtmosphereRadius
            );

            BuildAtmosphereBasedOnBodyType(type);
        } 
        else if (type == (ushort)cb_bodytype.TerranMoon)
        {
            data.tConfig.equitorialRadius = Random.Range(
                cb_solarsystem.Instance.minimumTerranLunarSurfaceRadius,
                cb_solarsystem.Instance.maximumTerranLunarSurfaceRadius
            );

            data.hasAtmosphere = false;
            data.tConfig.atmosphericRadius = -1;
        } 
        else if (type == (ushort)cb_bodytype.JovianMoon)
        {
            data.tConfig.equitorialRadius = Random.Range(
                cb_solarsystem.Instance.minimumJovianLunarSurfaceRadius,
                cb_solarsystem.Instance.maximumJovianLunarSurfaceRadius
            );

            data.hasAtmosphere = false;
            data.tConfig.atmosphericRadius = -1;
        }
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
            data.pConfig.iPosition = Vector3.zero;
            data.pConfig.iVelocity = Vector3.zero;

            pose.data.localPosition = new DoubleVector3(data.pConfig.iPosition);
            pose.data.velocity = new DoubleVector3(data.pConfig.iVelocity);
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

        pose.data.localPosition = new DoubleVector3(data.pConfig.iPosition);
        pose.data.velocity = new DoubleVector3(data.pConfig.iVelocity);
    }
}
