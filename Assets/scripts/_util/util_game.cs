using UnityEngine;

// TODO: store all constants used here in some sort of data structure 
// (distance and temp change thresholds, mainly)

public class util_game
{
    // -3 to 3, used for displaying temp change to the player
    // (takes in deg C per second)
    public static int FormatTemperatureChange(float val)
    {
        // just a bunch of if statements for now
        // why complicate life?
        if (val < -10f)
        {
            return -3;
        } else if (val < -5f)
        {
            return -2;
        } else if (val < -0.1f)
        {
            return -1;
        } else if (val < 0.1f)
        {
            return 0;
        } else if (val < 5f)
        {
            return 1;
        } else if (val < 10f)
        {
            return 2;
        } else
        {
            return 3;
        }
    }

    public static string FormatDistance(num_precise distInMeters)
    {
        // the threshold for distance readouts is four digits
        // 9999.0m -> 10.0km
        // 9999.0km -> 10.0Mm
        if (distInMeters.raw < new num_precise(10000).raw)
        {
            // meters
            return util_math.RoundToInterval(distInMeters.AsDouble(), 0.1f).ToString() + "m";
        } else if (distInMeters.raw < new num_precise(10000000).raw)
        {
            // kilometers
            return util_math.RoundToInterval(distInMeters.AsDouble() / 1000f, 0.1f).ToString() + "km";
        } else if (distInMeters.raw < new num_precise(new System.Numerics.BigInteger(10000000000 * 10000)).raw)
        {
            // megameters
            return util_math.RoundToInterval(distInMeters.AsDouble() / 1000000f, 0.1f).ToString() + "Mm";
        } else if (distInMeters.raw < new num_precise(new System.Numerics.BigInteger(10000000000000 * 10000)).raw)
        {
            // gigameters
            return util_math.RoundToInterval(distInMeters.AsDouble() / 1000000000f, 0.1f).ToString() + "Gm";
        }

        // should never get here
        return "err";
    }

    public static string FormatDistance(double distInMeters)
    {
        // the threshold for distance readouts is four digits
        // 9999.0m -> 10.0km
        // 9999.0km -> 10.0Mm
        if (distInMeters < 10000)
        {
            // meters
            return util_math.RoundToInterval(distInMeters, 0.01f) + "m";
        } else if (distInMeters < 10000000)
        {
            // kilometers
            return util_math.RoundToInterval(distInMeters / 1000f, 0.01f) + "km";
        } else if (distInMeters < 10000000000)
        {
            // megameters
            return util_math.RoundToInterval(distInMeters / 1000000f, 0.01f) + "Mm";
        } else if (distInMeters < 10000000000000)
        {
            // gigameters
            return util_math.RoundToInterval(distInMeters / 1000000000f, 0.01f) + "Gm";
        }

        // should never get here
        return "err";
    }
    public static string FormatDistance(float distInMeters)
    {
        // the threshold for distance readouts is four digits
        // 9999.0m -> 10.0km
        // 9999.0km -> 10.0Mm
        if (distInMeters < 10000)
        {
            // meters
            return util_math.RoundToInterval(distInMeters, 0.1f) + "m";
        } else if (distInMeters < 10000000)
        {
            // kilometers
            return util_math.RoundToInterval(distInMeters / 1000f, 0.1f) + "km";
        } else if (distInMeters < 10000000000)
        {
            // megameters
            return util_math.RoundToInterval(distInMeters / 1000000f, 0.1f) + "Mm";
        } else if (distInMeters < 10000000000000)
        {
            // gigameters
            return util_math.RoundToInterval(distInMeters / 1000000000f, 0.1f) + "Gm";
        }

        // should never get here
        return "err";
    }

    public static string FormatRawDistance(num_precise distInMeters)
    {
        return FormatDistance(distInMeters.Mul(1f / WorldData.universalScaleFactor));
    }

    public static string FormatRawDistance(float distInMeters)
    {
        return FormatDistance(distInMeters / WorldData.universalScaleFactor);
    }

    public static string FormatRawDistance(double distInMeters)
    {
        return FormatDistance(distInMeters / WorldData.universalScaleFactor);
    }
}
