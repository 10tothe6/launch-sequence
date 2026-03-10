using System.Collections.Generic;
using UnityEngine;

// TODO: split this script into a terrain config and an atmosphere config???

// instead of having one big config script, I'm splitting it up into a few
// this part of the config handles stuff like atmospheric composition

// this script is actually updated in-game, as things like atmospheric composition can change thanks to the player
[System.Serializable]
public class cbt_config : MonoBehaviour
{
    // **************** BASIC ****************
    // sizes
    public float equitorialRadius;
    public float atmosphericRadius;
    // clouds
    public float maxCloudRadius;
    public float minCloudRadius;
    // ********************************




    // **************** ATMOSPHERE DATA ****************
    // ids referencing gasses
    public List<int> defaultAtmosphereGasTypes; // storing both of these in a custom class seemed like too much work
    public List<float> defaultAtmosphereGasAmounts; // <-- IN m^3

    public List<int> currentAtmosphereGasTypes;
    public List<float> currentAtmosphereGasAmounts;
    // updated when the lists are modified
    // kept in a var so it doesn't need to be calculated all the time
    public float totalAtmosphereSize; // total amount of gas, in m^3
    // ********************************



    // **************** ATMOSPHERE VISUALS ****************
    // atmo shader vars

    // changes the density curve of the atmo, see sebastian lague's video for details
    public float densityFalloff;
    // changing how well light gets through the atmo
    // ACTUALLY DOES WHAT YOU'D THINK IT DOES, unlike previous attempt
    public float densityMultiplier;
    // how well light makes it through the atmosphere (de facto surface brightness)
    public float luminance;
    // brightness multiplier for the whole atmo, helps to reduce whiteness caused by scattering
    public float externalBrightness;
    // brightness multiplier (external/internal) for clouds
    public float cloudBrightness;
    // can we just accept that these are different vars and just leave it at that, no idea what the diff is
    public float scatterFactor;
    public float scatterStrength;

    // changes not only the color of the atmo but also the light scattering
    public Vector3 waveLengths;
    // ********************************

    public float GetGasPercent(int type)
    {
        for (int i = 0; i < currentAtmosphereGasTypes.Count; i++)
        {
            if (currentAtmosphereGasTypes[i] == type)
            {
                return currentAtmosphereGasAmounts[i] / totalAtmosphereSize;
            }
        }

        return 0; // no gas present, so concentration is 0%
    }

    // for when a gas is released into the atmosphere
    public void AddGas(int type, float amt)
    {
        int indexInList = -1;
        for (int i = 0; i < currentAtmosphereGasTypes.Count; i++)
        {
            if (currentAtmosphereGasTypes[i] == type)
            {
                indexInList = i;
                break;
            }
        }

        if (indexInList == -1)
        {
            currentAtmosphereGasTypes.Add(type);
            currentAtmosphereGasAmounts.Add(amt);
        } else
        {
            currentAtmosphereGasAmounts[indexInList] += amt;
        }

        RecalculateTotalAtmosphereSize();
    }

    public void RecalculateTotalAtmosphereSize()
    {
        totalAtmosphereSize = 0;
        for (int i = 0; i < currentAtmosphereGasAmounts.Count; i++)
        {
            totalAtmosphereSize += currentAtmosphereGasAmounts[i];
        }
    }
}
